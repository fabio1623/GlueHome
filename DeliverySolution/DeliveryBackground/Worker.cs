using DeliveryBackground.Services;

namespace DeliveryBackground;

public class Worker : BackgroundService
{
    private readonly IDeliveryService _deliveryService;
    private readonly ILogger<Worker> _logger;

    public Worker(IDeliveryService deliveryService, ILogger<Worker> logger)
    {
        _deliveryService = deliveryService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
            var orderNumbers = await _deliveryService.GetExpiredDeliveries();
            await _deliveryService.ExpireDeliveries(orderNumbers);
            await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
        }
    }
}