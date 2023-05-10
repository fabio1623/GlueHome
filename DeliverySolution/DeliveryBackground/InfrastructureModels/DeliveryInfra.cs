namespace DeliveryBackground.InfrastructureModels;

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
}