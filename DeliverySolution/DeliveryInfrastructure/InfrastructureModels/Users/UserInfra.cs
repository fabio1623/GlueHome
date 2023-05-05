using DeliveryDomain.DomainModels.Users;
using DeliveryInfrastructure.Exceptions;

namespace DeliveryInfrastructure.InfrastructureModels.Users;

public class UserInfra
{
    public int? Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Username { get; init; }
    public RoleInfra? RoleInfra { get; set; }
    public string? PasswordHash { get; init; }

    public UserDomain ToDomain()
    {
        if (!Enum.TryParse<RoleDomain>(RoleInfra.ToString(), true, out var parsedRoleDomain))
            throw new InfrastructureException($"Could not parse RoleInfra '{RoleInfra.ToString()}'");
            
        return new UserDomain
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Username = Username,
            RoleDomain = parsedRoleDomain,
            PasswordHash = PasswordHash
        };
    }
}