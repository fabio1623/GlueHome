using AutoMapper;
using DeliveryDomain.DomainEnums;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.Enums;
using DeliveryInfrastructure.Exceptions;
using DeliveryInfrastructure.InfrastructureModels;
using MongoDB.Driver;

namespace DeliveryInfrastructure.Services;

public class DeliveryService : BaseMongoDbService<DeliveryDomain.DomainModels.DeliveryDomain, CreateDeliveryDomain, UpdateDeliveryDomain, DeliveryInfra>, IDeliveryService
{
    protected override string CollectionName => "Deliveries";
    
    
    public DeliveryService(IMongoDbConfiguration mongoDbConfiguration, IMapper mapper) : base(mongoDbConfiguration, mapper)
    {
    }

    protected override CreateIndexModel<DeliveryInfra> GetCreateIndexModel()
    {
        var indexKeysDefinition = Builders<DeliveryInfra>
            .IndexKeys
            .Ascending(x => x.Order!.OrderNumber);

        var createIndexOptions = new CreateIndexOptions { Unique = true };
        
        return new CreateIndexModel<DeliveryInfra>(indexKeysDefinition, createIndexOptions);
    }

    protected override UpdateDefinition<DeliveryInfra> GetUpdateDefinition(UpdateDeliveryDomain? domainModelUpdate)
    {
        var mappedStateInfra = Mapper.Map<StateInfra?>(domainModelUpdate?.State);
        if (mappedStateInfra == null)
            throw new InfrastructureException($"Could not parse {nameof(StateDomain)} '{domainModelUpdate?.State?.ToString()}'.");
        
        var currentDateTime = DateTime.UtcNow;

        return Builders<DeliveryInfra>
            .Update
            .Set(x => x.State, mappedStateInfra)
            .Set(x => x.UpdatedAt, currentDateTime);
    }
}