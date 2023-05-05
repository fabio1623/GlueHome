using DeliveryDomain.DomainModels.Users;

namespace DeliveryApi.Models.Users;

public class User
{
    public int? Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Username { get; init; }
    public string? Role { get; set; }

    public User(UserDomain? userDomain)
    {
        Id = userDomain?.Id;
        FirstName = userDomain?.FirstName;
        LastName = userDomain?.LastName;
        Username = userDomain?.Username;
        Role = userDomain?.RoleDomain?.ToString();
    }
}