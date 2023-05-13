namespace DeliveryDomain.Interfaces.Configurations;

public interface IWorkerConfiguration
{
    double? DelayInMinutes { get; set; }
}