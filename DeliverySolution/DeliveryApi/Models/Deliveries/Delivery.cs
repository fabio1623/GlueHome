namespace DeliveryApi.Models.Deliveries;

public class Delivery
{
    public string? State { get; set; }
    public AccessWindow? AccessWindow { get; set; }
    public Recipient? Recipient { get; set; }
    public Order? Order { get; set; }
    
    public Delivery(DeliveryDomain.DomainModels.Deliveries.DeliveryDomain? deliveryDomain)
    {
        State = deliveryDomain?.State?.ToString();
        AccessWindow = new AccessWindow(deliveryDomain?.AccessWindow);
        Recipient = new Recipient(deliveryDomain?.Recipient);
        Order = new Order(deliveryDomain?.Order);
    }
}