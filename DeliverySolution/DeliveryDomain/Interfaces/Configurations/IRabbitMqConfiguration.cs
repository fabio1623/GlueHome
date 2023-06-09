namespace DeliveryDomain.Interfaces.Configurations;

public interface IRabbitMqConfiguration
{
    string? HostName { get; set; }
    string? UserName { get; set; }
    string? Password { get; set; }
    string? ExchangeName { get; set; }
    double? RetryDelayInMinutes { get; set; }
}