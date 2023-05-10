namespace DeliveryBackground.Configurations;

public interface IElasticSearchConfiguration
{
    string? ConnectionString { get; set; }
}