using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryDomain.Interfaces.Businesses;

public interface IDeliveryBusiness
{
    Task Create(CreateDeliveryDomain? deliveryDomain, CancellationToken cancellationToken);
    Task<DomainModels.Deliveries.DeliveryDomain?> Get(string? orderNumber, CancellationToken cancellationToken);
    Task Approve(string? orderNumber, CancellationToken cancellationToken);
    Task Complete(string? orderNumber, CancellationToken cancellationToken);
    Task Cancel(string? orderNumber, CancellationToken cancellationToken);
    Task Delete(string? orderNumber, CancellationToken cancellationToken);
}