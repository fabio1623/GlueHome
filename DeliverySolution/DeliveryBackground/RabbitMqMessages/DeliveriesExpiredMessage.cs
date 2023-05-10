namespace DeliveryBackground.RabbitMqMessages;

public class DeliveriesExpiredMessage
{
    public IEnumerable<string>? OrderNumbers { get; set; }
}