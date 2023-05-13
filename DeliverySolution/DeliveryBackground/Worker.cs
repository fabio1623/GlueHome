using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Initializers;

namespace DeliveryBackground;

public class Worker : BackgroundService
{
    private readonly IWorkerConfiguration _workerConfiguration;
    private readonly IDeliveriesInitializer _deliveriesInitializer;
    private readonly ILogger<Worker> _logger;

    public Worker(IWorkerConfiguration workerConfiguration, IDeliveriesInitializer deliveriesInitializer, ILogger<Worker> logger)
    {
        _workerConfiguration = workerConfiguration;
        _deliveriesInitializer = deliveriesInitializer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
            
            await _deliveriesInitializer.ExpireDeliveries(cancellationToken);
            
            var delay = TimeSpan.FromMinutes(_workerConfiguration.DelayInMinutes ?? 60);
            _logger.LogInformation("Worker finished at: {time}. Waiting {delay} minutes.", DateTimeOffset.UtcNow, delay.TotalMinutes);
            
            await Task.Delay(delay, cancellationToken);
        }
    }
}