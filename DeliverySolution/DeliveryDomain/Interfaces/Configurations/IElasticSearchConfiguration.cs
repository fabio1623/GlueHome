namespace DeliveryDomain.Interfaces.Configurations;

public interface IElasticSearchConfiguration
{
    string? ConnectionString { get; set; }
}