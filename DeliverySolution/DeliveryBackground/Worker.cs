using DeliveryDomain.Interfaces.Initializers;

namespace DeliveryBackground;

public class Worker : BackgroundService
{
    private readonly IDeliveriesInitializer _deliveriesInitializer;
    private readonly ILogger<Worker> _logger;

    public Worker(IDeliveriesInitializer deliveriesInitializer, ILogger<Worker> logger)
    {
        _deliveriesInitializer = deliveriesInitializer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
            
            await _deliveriesInitializer.ExpireDeliveries(cancellationToken);
            
            var delay = TimeSpan.FromSeconds(30);
            _logger.LogInformation("Worker finished at: {time}. Waiting {delay}min.", DateTimeOffset.UtcNow, delay.TotalMinutes);
            
            await Task.Delay(delay, cancellationToken);
        }
    }
}