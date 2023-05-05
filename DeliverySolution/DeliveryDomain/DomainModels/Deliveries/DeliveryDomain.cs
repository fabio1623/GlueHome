namespace DeliveryDomain.DomainModels.Deliveries;

public class DeliveryDomain
{
    public StateDomain? State { get; set; }
    public AccessWindowDomain? AccessWindow { get; set; }
    public RecipientDomain? Recipient { get; set; }
    public OrderDomain? Order { get; set; }
}