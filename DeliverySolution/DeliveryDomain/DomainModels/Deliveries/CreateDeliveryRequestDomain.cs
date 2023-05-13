using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels.Deliveries;

public class CreateDeliveryRequestDomain
{
    public StateDomain? State => StateDomain.Created;
    public CreateDeliveryAccessWindowDomain? AccessWindow { get; set; }
    public CreateDeliveryRecipientDomain? Recipient { get; set; }
    public CreateDeliveryOrderDomain? Order { get; set; }
    
    public class CreateDeliveryAccessWindowDomain
    {
        public DateTime? StartTime => DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
    }
    
    public class CreateDeliveryRecipientDomain
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    
    public class CreateDeliveryOrderDomain
    {
        public string? OrderNumber { get; set; }
        public string? Sender { get; set; }
    }
}