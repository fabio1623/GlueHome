namespace DeliveryBackground.Services;

public interface IDeliveryService
{
    Task<IEnumerable<string>> GetExpiredDeliveries();
    Task ExpireDeliveries(IEnumerable<string> orderNumbers);
}