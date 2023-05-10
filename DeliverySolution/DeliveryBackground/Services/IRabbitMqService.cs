namespace DeliveryBackground.Services;

public interface IRabbitMqService
{
    Task ProduceMessage<T>(T message);
}