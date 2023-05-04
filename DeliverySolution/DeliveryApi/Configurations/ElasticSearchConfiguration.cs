using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryApi.Configurations;

public class ElasticSearchConfiguration : IElasticSearchConfiguration
{
    public string? ConnectionString { get; set; }
}