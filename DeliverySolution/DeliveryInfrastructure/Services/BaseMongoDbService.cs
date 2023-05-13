using System.Text.Json;
using AutoMapper;
using DeliveryDomain.DomainModels;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.Exceptions;
using DeliveryInfrastructure.InfrastructureModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeliveryInfrastructure.Services;

public abstract class BaseMongoDbService<TDomainModel, TDomainModelCreate, TDomainModelUpdate, TInfrastructureModel> : IBaseMongoDbService<TDomainModel, TDomainModelCreate, TDomainModelUpdate>
    where TDomainModel : BaseModelDomain
    where TInfrastructureModel : BaseModelInfrastructure
{
    protected readonly IMapper Mapper;
    protected readonly IMongoCollection<TInfrastructureModel> MongoCollection;
    
    protected abstract string? CollectionName { get; }
    protected abstract CreateIndexModel<TInfrastructureModel> GetCreateIndexModel();
    protected abstract UpdateDefinition<TInfrastructureModel> GetUpdateDefinition(TDomainModelUpdate? domainModelUpdate);
    
    protected static UpdateDefinition<TInfrastructureModel> DefaultUpdateDefinition =>
        Builders<TInfrastructureModel>.Update.Set(x => x.UpdatedAt, DateTime.UtcNow);
    
    protected BaseMongoDbService(IMongoDbConfiguration mongoDbConfiguration, IMapper mapper)
    {
        Mapper = mapper;
        var client = new MongoClient(mongoDbConfiguration.ConnectionString);
        var database = client.GetDatabase(mongoDbConfiguration.Database);
        
        MongoCollection = database.GetCollection<TInfrastructureModel>(CollectionName);
        CreateIndexes();
    }
    
    public async Task<PagedListDomain<TDomainModel?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken)
    {
        var filterDefinition = Builders<TInfrastructureModel>
            .Filter
            .Empty;

        if (requestedPage == null || pageSize == null)
            throw new InfrastructureException($"'{nameof(requestedPage)}': '{requestedPage}' and '{nameof(pageSize)}': '{pageSize}' cannot be null.");

        return await FindPaged(filterDefinition, requestedPage.Value, pageSize.Value, cancellationToken);
    }

    public async Task<TDomainModel?> Get(string? id, CancellationToken cancellationToken)
    {
        var filterDefinition = GetIdFilterDefinition(id);

        return await FindOne(filterDefinition, cancellationToken);
    }

    public async Task<TDomainModel?> Create(TDomainModelCreate? domainModelCreate, CancellationToken cancellationToken)
    {
        var infrastructureModel = Mapper.Map<TInfrastructureModel>(domainModelCreate);
        
        var currentDateTime = DateTime.UtcNow;
        infrastructureModel.CreatedAt = currentDateTime;
        infrastructureModel.UpdatedAt = currentDateTime;

        return await InsertOne(infrastructureModel, cancellationToken);
    }

    public async Task Update(string? id, TDomainModelUpdate? domainModelUpdate, CancellationToken cancellationToken)
    {
        var updateDefinition = GetUpdateDefinition(domainModelUpdate);

        await UpdateOne(id, updateDefinition, cancellationToken);
    }

    public async Task Delete(string? id, CancellationToken cancellationToken)
    {
        var filterDefinition = GetIdFilterDefinition(id);
        
        var result = await MongoCollection.DeleteOneAsync(filterDefinition, cancellationToken);

        if (!result.IsAcknowledged)
            throw new InfrastructureException($"Something went wrong when deleting {typeof(TInfrastructureModel).Name} '{id}'.");

        if(result.DeletedCount == 0)
            throw new InfrastructureException($"{typeof(TInfrastructureModel).Name} '{id}' does not exist.");
    }
    
    private async Task<PagedListDomain<TDomainModel?>> FindPaged(FilterDefinition<TInfrastructureModel> filterDefinition, int requestedPage, int pageSize, CancellationToken cancellationToken)
    {
        const string countFacetName = "count";
        const string resultsFacetName = "results";

        var countFacet = AggregateFacet.Create(countFacetName, PipelineDefinition<TInfrastructureModel, AggregateCountResult>.Create(new[] { PipelineStageDefinitionBuilder.Count<TInfrastructureModel>() }));
        var resultFacet = AggregateFacet.Create(resultsFacetName, PipelineDefinition<TInfrastructureModel, TInfrastructureModel>.Create(new[] { PipelineStageDefinitionBuilder.Skip<TInfrastructureModel>((requestedPage - 1) * pageSize), PipelineStageDefinitionBuilder.Limit<TInfrastructureModel>(pageSize) }));
        
        var facets = new List<AggregateFacet<TInfrastructureModel>>
        {
            countFacet,
            resultFacet
        };

        var aggregation = await MongoCollection
            .Aggregate()
            .Match(filterDefinition)
            .Facet(facets)
            .ToListAsync(cancellationToken);

        var count = aggregation
            .FirstOrDefault()?
            .Facets?
            .FirstOrDefault(x => x.Name == countFacetName)?
            .Output<AggregateCountResult>()?
            .FirstOrDefault()?
            .Count ?? 0;
        
        var totalPages = (int)Math.Ceiling((double)count / pageSize);
        
        if (requestedPage > totalPages)
            throw new InfrastructureException($"Page '{requestedPage}' does not exist. Maximum page is '{totalPages}'.");

        var data = aggregation
            .FirstOrDefault()?
            .Facets?
            .FirstOrDefault(x => x.Name == resultsFacetName)?
            .Output<TInfrastructureModel>();

        return new PagedListDomain<TDomainModel?>
        {
            CurrentPage = requestedPage,
            TotalPages = totalPages,
            TotalResults = count,
            Results = Mapper.Map<IEnumerable<TDomainModel?>>(data)
        };
    }
    
    private FilterDefinition<TInfrastructureModel> GetIdFilterDefinition(string? id)
    {
        var objectId = Mapper.Map<ObjectId>(id);

        return Builders<TInfrastructureModel>
            .Filter
            .Eq(x => x.Id, objectId);
    }

    protected async Task<TDomainModel?> FindOne(FilterDefinition<TInfrastructureModel> filterDefinition, CancellationToken cancellationToken)
    {
        var infrastructureModel = await MongoCollection
            .Find(filterDefinition)
            .FirstOrDefaultAsync(cancellationToken);

        return Mapper.Map<TDomainModel?>(infrastructureModel);
    }
    
    private async Task<TDomainModel?> InsertOne(TInfrastructureModel? infrastructureModel, CancellationToken cancellationToken)
    {
        if (infrastructureModel == null)
            throw new InfrastructureException($"{typeof(TInfrastructureModel).Name} is null. Could not insert.");
        
        try
        {
            var options = new InsertOneOptions();
            await MongoCollection.InsertOneAsync(infrastructureModel, options, cancellationToken);
        }
        catch (Exception e)
        {
            throw new InfrastructureException($"Could not insert {typeof(TInfrastructureModel).Name}: {JsonSerializer.Serialize(infrastructureModel, infrastructureModel.GetType())}.", e);
        }
        
        return Mapper.Map<TDomainModel?>(infrastructureModel);
    }

    private async Task UpdateOne(string? id, UpdateDefinition<TInfrastructureModel> updateDefinition, CancellationToken cancellationToken)
    {
        var filterDefinition = GetIdFilterDefinition(id);

        var updateOptions = new UpdateOptions
        {
            IsUpsert = false
        };
        
        var result = await MongoCollection.UpdateOneAsync(filterDefinition, updateDefinition, updateOptions, cancellationToken);

        if (!result.IsAcknowledged || result.MatchedCount != result.ModifiedCount)
            throw new InfrastructureException($"Something went wrong when updating {typeof(TInfrastructureModel).Name}.");

        if (result is { IsModifiedCountAvailable: true, ModifiedCount: 0 })
            throw new InfrastructureException($"{typeof(TInfrastructureModel).Name} '{id}' does not exist.");
    }
    
    private void CreateIndexes()
    {
        var indexModel = GetCreateIndexModel();
        MongoCollection
            .Indexes
            .CreateOne(indexModel);
    }
}