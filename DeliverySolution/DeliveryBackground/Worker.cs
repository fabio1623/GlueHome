using DeliveryBackground.RabbitMqMessages;
using DeliveryDomain.Interfaces.Services;

namespace DeliveryBackground;

public class Worker : BackgroundService
{
    private readonly IDeliveryService _deliveryService;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<Worker> _logger;

    public Worker(IDeliveryService deliveryService, IRabbitMqService rabbitMqService, ILogger<Worker> logger)
    {
        _deliveryService = deliveryService;
        _rabbitMqService = rabbitMqService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
            var orderNumbers = (await _deliveryService.GetExpiredDeliveries()).ToList();
            await _deliveryService.ExpireDeliveries(orderNumbers);
            await ProduceDeliveryExpiry(orderNumbers);
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }

    private async Task ProduceDeliveryExpiry(IEnumerable<string> orderNumbers)
    {
        var deliveriesExpiredMessage = new DeliveriesExpiredMessage
        {
            OrderNumbers = orderNumbers
        };
        await _rabbitMqService.ProduceMessage(deliveriesExpiredMessage);
    }
}