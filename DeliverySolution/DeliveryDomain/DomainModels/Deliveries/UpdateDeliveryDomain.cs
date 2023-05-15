using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels.Deliveries;

public class UpdateDeliveryDomain
{
    public StateDomain? State { get; set; }
}