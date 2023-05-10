using System.Text;
using System.Text.Json;
using DeliveryBackground.Configurations;
using RabbitMQ.Client;

namespace DeliveryBackground.Services;

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private readonly IRabbitMqConfiguration _rabbitMqConfiguration;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    
    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(IRabbitMqConfiguration rabbitMqConfiguration, IConnectionFactory connectionFactory, ILogger<RabbitMqService> logger)
    {
        _rabbitMqConfiguration = rabbitMqConfiguration;
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(
            rabbitMqConfiguration.ExchangeName, 
            ExchangeType.Direct);
        
        _logger = logger;
    }
    
    public Task ProduceMessage<T>(T message)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.ContentType = "application/json";
            basicProperties.Type = typeof(T).Name;
            basicProperties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
            _channel.BasicPublish(
                _rabbitMqConfiguration.ExchangeName,
                "*",
                basicProperties,
                body);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while publishing message.");
        }
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        try
        {
            _channel.Close();
            _channel.Dispose();
            _connection.Close();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot dispose RabbitMQ channel or connection.");
        }
    }
}