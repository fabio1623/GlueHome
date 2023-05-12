namespace DeliveryDomain.DomainModels.BrokerMessages;

public class DeliveryUpdatedMessage
{
    public string? OrderNumber { get; set; }
    public string? NewState { get; set; }
}