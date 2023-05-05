namespace DeliveryDomain.DomainModels.RabbitMqMessages;

public class DeliveryDeletedMessage
{
    public string? OrderNumber { get; set; }
}