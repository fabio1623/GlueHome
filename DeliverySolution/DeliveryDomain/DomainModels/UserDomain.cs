using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels;

public class UserDomain : BaseModelDomain
{
    public string? Username { get; init; }
    public RoleDomain? Role { get; set; }
    public string? PasswordHash { get; init; }
}