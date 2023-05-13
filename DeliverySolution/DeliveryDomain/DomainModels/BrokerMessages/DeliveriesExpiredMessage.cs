namespace DeliveryDomain.DomainModels.BrokerMessages;

public class DeliveriesExpiredMessage
{
    public IEnumerable<string>? DeliveryIds { get; set; }
}