namespace DeliveryDomain.DomainModels.BrokerMessages;

public class DeliveryUpdatedMessage
{
    public string? DeliveryId { get; set; }
    public string? NewState { get; set; }
}