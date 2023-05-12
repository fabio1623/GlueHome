namespace DeliveryDomain.Interfaces.Initializers;

public interface IDeliveriesInitializer
{
    Task Initialize(CancellationToken cancellationToken);
    Task ExpireDeliveries(CancellationToken cancellationToken);
}