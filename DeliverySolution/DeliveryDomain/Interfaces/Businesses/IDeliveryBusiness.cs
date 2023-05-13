using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryDomain.Interfaces.Businesses;

public interface IDeliveryBusiness
{
    Task<DomainModels.DeliveryDomain?> Create(CreateDeliveryRequestDomain? deliveryDomain, CancellationToken cancellationToken);
    Task<PagedListDomain<DomainModels.DeliveryDomain?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken);
    Task<DomainModels.DeliveryDomain?> Get(string? deliveryId, CancellationToken cancellationToken);
    Task Approve(string? deliveryId, CancellationToken cancellationToken);
    Task Complete(string? deliveryId, CancellationToken cancellationToken);
    Task Cancel(string? deliveryId, CancellationToken cancellationToken);
    Task Delete(string? deliveryId, CancellationToken cancellationToken);
}