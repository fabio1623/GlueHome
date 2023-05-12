namespace DeliveryApi.Models;

public class Delivery : BaseModel
{
    public string? State { get; set; }
    public DeliveryAccessWindow? AccessWindow { get; set; }
    public DeliveryRecipient? Recipient { get; set; }
    public DeliveryOrder? Order { get; set; }

    public class DeliveryAccessWindow
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
    
    public class DeliveryRecipient
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    
    public class DeliveryOrder
    {
        public string? OrderNumber { get; set; }
        public string? Sender { get; set; }
    }
}