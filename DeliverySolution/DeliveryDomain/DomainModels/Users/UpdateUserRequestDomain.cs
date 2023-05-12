using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels.Users;

public class UpdateUserRequestDomain
{
    public RoleDomain? RoleDomain { get; set; }
}