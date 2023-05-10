using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryDomain.Interfaces.Services;

public interface IDeliveryService
{
    Task Create(CreateDeliveryDomain? deliveryDomain, CancellationToken cancellationToken);
    Task<DomainModels.Deliveries.DeliveryDomain?> Get(string? orderNumber, CancellationToken cancellationToken);
    Task Update(string? orderNumber, DeliveryUpdateDomain? deliveryUpdateDomain, CancellationToken cancellationToken);
    Task Delete(string? orderNumber, CancellationToken cancellationToken);
    
    Task<IEnumerable<string>> GetExpiredDeliveries();
    Task ExpireDeliveries(IEnumerable<string> orderNumbers);
}