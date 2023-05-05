namespace DeliveryDomain.Interfaces.Services;

public interface IRabbitMqService
{
    Task ProduceMessage<T>(T message);
}