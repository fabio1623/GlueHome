using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliveryInfrastructure.InfrastructureModels
{
    public abstract class BaseModelInfrastructure
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions]
        public DateTime CreatedAt { get; set; }

        [BsonDateTimeOptions]
        public DateTime UpdatedAt { get; set; }
    }
}