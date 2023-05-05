namespace DeliveryDomain.DomainModels.Deliveries;

public class CreateDeliveryDomain
{
    public DateTime? EndTime { get; set; }
    public RecipientDomain? Recipient { get; set; }
    public OrderDomain? Order { get; set; }
}