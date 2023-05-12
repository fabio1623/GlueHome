using DeliveryInfrastructure.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliveryInfrastructure.InfrastructureModels;

public class DeliveryInfra : BaseModelInfrastructure
{
    [BsonRepresentation(BsonType.String)]
    public StateInfra? State { get; set; }
    public DeliveryAccessWindowInfra? AccessWindow { get; set; }
    public DeliveryRecipientInfra? Recipient { get; set; }
    public DeliveryOrderInfra? Order { get; set; }
    
    public class DeliveryAccessWindowInfra
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    
    public class DeliveryRecipientInfra
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    
    public class DeliveryOrderInfra
    {
        public string? OrderNumber { get; set; }
        public string? Sender { get; set; }
    }
}