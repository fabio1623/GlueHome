using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryApi.Configurations
{
    public class MongoDbConfiguration : IMongoDbConfiguration
    {
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
    }
}