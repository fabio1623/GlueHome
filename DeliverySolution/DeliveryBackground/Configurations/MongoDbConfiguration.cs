using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryBackground.Configurations
{
    public class MongoDbConfiguration : IMongoDbConfiguration
    {
        public string? ConnectionString { get; set; }
        public string? Database { get; set; }
    }
}