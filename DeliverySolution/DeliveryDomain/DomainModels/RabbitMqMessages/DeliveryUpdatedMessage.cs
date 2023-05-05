using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryDomain.DomainModels.RabbitMqMessages;

public class DeliveryUpdatedMessage
{
    public string? OrderNumber { get; set; }
    public string? NewState { get; set; }
}