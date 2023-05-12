namespace DeliveryDomain.DomainModels.BrokerMessages;

public class DeliveriesExpiredMessage
{
    public IEnumerable<string>? OrderNumbers { get; set; }
}