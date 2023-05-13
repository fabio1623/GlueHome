using AutoMapper;
using DeliveryDomain.DomainModels.BrokerMessages;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Initializers;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.Enums;
using DeliveryInfrastructure.InfrastructureModels;
using DeliveryInfrastructure.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeliveryInfrastructure.Initializers;

public class DeliveriesInitializer : DeliveryService, IDeliveriesInitializer
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<DeliveriesInitializer> _logger;
    
    public DeliveriesInitializer(IMongoDbConfiguration mongoDbConfiguration, IMapper mapper, IRabbitMqService rabbitMqService, ILogger<DeliveriesInitializer> logger) : base(mongoDbConfiguration, mapper)
    {
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    public async Task Initialize(CancellationToken cancellationToken)
    {
        await SeedDeliveries(1000, cancellationToken);
    }

    public async Task ExpireDeliveries(CancellationToken cancellationToken)
    {
        var filterDefinitionBuilder = Builders<DeliveryInfra>.Filter;
        
        var filter = filterDefinitionBuilder.And(
            filterDefinitionBuilder.Lt(x => x.AccessWindow!.EndTime, DateTime.UtcNow), 
            filterDefinitionBuilder.Eq(x => x.State, StateInfra.Created)
        );
        
        var findOptions = new FindOptions<DeliveryInfra, ObjectId>
        {
            Projection = new FindExpressionProjectionDefinition<DeliveryInfra, ObjectId>(x => x.Id), 
            BatchSize = 1000
        };

        try
        {
            using var cursor = await MongoCollection.FindAsync(filter, findOptions, cancellationToken);
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                var batch = cursor.Current?.ToList();
                if (batch?.Count is 0)
                    continue;
                
                _logger.LogInformation("Batch contains '{nbDeliveries}' deliveries.", batch?.Count);
                var nbUpdates = await UpdateDeliveriesState(batch, StateInfra.Expired, cancellationToken);
                _logger.LogInformation("'{nbUpdates}' deliveries updated in current batch.", nbUpdates);
                
                await ProduceDeliveryExpiry(batch);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning("Something went wrong when expiring deliveries. Exception: {exception}", e);
        }
    }

    private async Task<long> UpdateDeliveriesState(IEnumerable<ObjectId>? ids, StateInfra state, CancellationToken cancellationToken)
    {
        if (ids == null)
            return 0;
        
        var filter = Builders<DeliveryInfra>.Filter.In(x => x.Id, ids);

        var update = Builders<DeliveryInfra>.Update
            .Set(x => x.State, state)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var updateOptions = new UpdateOptions();
        var result = await MongoCollection.UpdateManyAsync(filter, update, updateOptions, cancellationToken);
        
        return result.ModifiedCount;
    }

    private async Task ProduceDeliveryExpiry(IEnumerable<ObjectId>? objectIds)
    {
        var ids = Mapper.Map<IEnumerable<string>>(objectIds);
        var deliveriesExpiredMessage = new DeliveriesExpiredMessage
        {
            DeliveryIds = ids
        };
        await _rabbitMqService.ProduceMessage(deliveriesExpiredMessage);
    }

    private async Task SeedDeliveries(int count, CancellationToken cancellationToken)
    {
        var deliveries = new List<DeliveryInfra>();
        var random = new Random();
    
        for (var i = 0; i < count; i++)
        {
            var delivery = CreateDelivery(random, i);
            deliveries.Add(delivery);
        }
    
        try
        {
            var insertManyOptions = new InsertManyOptions();
            await MongoCollection.InsertManyAsync(deliveries, insertManyOptions, cancellationToken);
            _logger.LogInformation("Deliveries seed done.");
        }
        catch (Exception e)
        {
            _logger.LogWarning("Deliveries seed not applied. Exception: {exception}", e);
        }
    }
    
    private static DeliveryInfra CreateDelivery(Random random, int i)
    {
        var currentDateTime = DateTime.UtcNow;
        var accessWindow = new DeliveryInfra.DeliveryAccessWindowInfra
        {
            StartTime = currentDateTime.AddDays(random.Next(-100, 0))
        };
    
        accessWindow.EndTime = accessWindow.StartTime.Value.AddDays(random.Next(1, 100));
    
        var recipient = new DeliveryInfra.DeliveryRecipientInfra
        {
            Name = $"Recipient {i}",
            Address = $"Address {i}",
            Email = $"recipient{i}@example.com",
            PhoneNumber = $"123456789{i}"
        };
    
        var order = new DeliveryInfra.DeliveryOrderInfra
        {
            OrderNumber = $"Order {i}",
            Sender = $"Sender {i}"
        };
    
        return new DeliveryInfra
        {
            AccessWindow = accessWindow,
            Recipient = recipient,
            Order = order,
            // Set the State property to Expired if the EndTime is in the past
            State = accessWindow.EndTime < currentDateTime
                ? StateInfra.Expired
                : (StateInfra)random.Next(Enum.GetValues(typeof(StateInfra)).Length),
            CreatedAt = currentDateTime,
            UpdatedAt = currentDateTime
        };
    }
}