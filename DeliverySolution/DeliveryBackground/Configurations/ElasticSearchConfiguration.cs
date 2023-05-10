using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryBackground.Configurations;

public class ElasticSearchConfiguration : IElasticSearchConfiguration
{
    public string? ConnectionString { get; set; }
}