using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels;

public class DeliveryDomain : BaseModelDomain
{
    public StateDomain? State { get; set; }
    public DeliveryAccessWindowDomain? AccessWindow { get; set; }
    public DeliveryRecipientDomain? Recipient { get; set; }
    public DeliveryOrderDomain? Order { get; set; }
    
    public class DeliveryAccessWindowDomain
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    
    public class DeliveryRecipientDomain
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    
    public class DeliveryOrderDomain
    {
        public string? OrderNumber { get; set; }
        public string? Sender { get; set; }
    }
}