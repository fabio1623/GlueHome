using DeliveryDomain.DomainModels.Users;
using DeliveryInfrastructure.Exceptions;

namespace DeliveryInfrastructure.InfrastructureModels.Users;

public class UserInfra
{
    public int? Id { get; set; }
    public string? Username { get; init; }
    public string? Role { get; set; }
    public string? PasswordHash { get; init; }

    public UserInfra()
    {
        
    }

    public UserDomain ToDomain()
    {
        if (!Enum.TryParse<RoleDomain>(Role, true, out var parsedRoleDomain))
            throw new InfrastructureException($"Could not parse Role '{Role}'.");
            
        return new UserDomain
        {
            Id = Id,
            Username = Username,
            RoleDomain = parsedRoleDomain,
            PasswordHash = PasswordHash
        };
    }

    public UserInfra(UserDomain? userDomain)
    {
        Id = userDomain?.Id;
        Username = userDomain?.Username;
        Role = userDomain?.RoleDomain?.ToString();
        PasswordHash = userDomain?.PasswordHash;
    }
}