using DeliveryDomain.DomainEnums;

namespace DeliveryDomain.DomainModels.Users;

public class CreateUserDomain
{
    public string? Username { get; init; }
    public RoleDomain? Role { get; set; }
    public string? PasswordHash { get; init; }
}