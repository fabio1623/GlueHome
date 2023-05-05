using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryInfrastructure.InfrastructureModels.Deliveries;

public class DeliveryInfra
{
    public string? OrderNumber { get; set; }
    public string? State { get; set; }
    public string? Sender { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientAddress { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientPhoneNumber { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public DeliveryInfra()
    {
        
    }

    public DeliveryInfra(CreateDeliveryDomain? createDeliveryDomain)
    {
        OrderNumber = createDeliveryDomain?.Order?.OrderNumber;
        State = StateInfra.Created.ToString();
        Sender = createDeliveryDomain?.Order?.Sender;
        RecipientName = createDeliveryDomain?.Recipient?.Name;
        RecipientAddress = createDeliveryDomain?.Recipient?.Address;
        RecipientEmail = createDeliveryDomain?.Recipient?.Email;
        RecipientPhoneNumber = createDeliveryDomain?.Recipient?.PhoneNumber;
        StartTime = DateTime.UtcNow;
        EndTime = createDeliveryDomain?.EndTime;
    }

    public DeliveryDomain.DomainModels.Deliveries.DeliveryDomain ToDomain()
    {
        var couldParseState = Enum.TryParse<StateDomain>(State, true, out var parsedState);
            
        return new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            State = couldParseState ? parsedState : null,
            AccessWindow = new AccessWindowDomain
            {
                StartTime = StartTime,
                EndTime = EndTime
            },
            Recipient = new RecipientDomain
            {
                Name = RecipientName,
                Address = RecipientAddress,
                Email = RecipientEmail,
                PhoneNumber = RecipientPhoneNumber
            },
            Order = new OrderDomain
            {
                OrderNumber = OrderNumber,
                Sender = Sender
            }
        };
    }
}