namespace DeliveryDomain.Interfaces.Configurations
{
    public interface IMongoDbConfiguration
    {
        string? ConnectionString { get; set; }
        string? Database { get; set; }
    }
}