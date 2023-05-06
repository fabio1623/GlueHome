using DeliveryDomain.DomainModels.Users;

namespace DeliveryApi.Models.Users;

public class User
{
    public int? Id { get; set; }
    public string? Username { get; init; }
    public string? Role { get; set; }

    public User(UserDomain? userDomain)
    {
        Id = userDomain?.Id;
        Username = userDomain?.Username;
        Role = userDomain?.RoleDomain?.ToString();
    }
}