using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels.Deliveries;

public class UpdateDeliveryRequestDomain
{
    public StateDomain? State { get; set; }
}