using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryBackground.Configurations;

public class RabbitMqConfiguration : IRabbitMqConfiguration
{
    public string? HostName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ExchangeName { get; set; }
    public double? RetryDelayInMinutes { get; set; }
}