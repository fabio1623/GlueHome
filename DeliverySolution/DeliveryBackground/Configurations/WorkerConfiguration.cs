using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryBackground.Configurations;

public class WorkerConfiguration : IWorkerConfiguration
{
    public double? DelayInMinutes { get; set; }
}