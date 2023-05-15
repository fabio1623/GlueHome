using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryDomain.Interfaces.Services;

public interface IDeliveryService : IBaseMongoDbService<DomainModels.DeliveryDomain, CreateDeliveryDomain, UpdateDeliveryDomain>
{
}