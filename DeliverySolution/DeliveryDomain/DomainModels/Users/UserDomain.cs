namespace DeliveryDomain.DomainModels.Users;

public class UserDomain
{
    public int? Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Username { get; init; }
    public RoleDomain? RoleDomain { get; set; }
    public string? PasswordHash { get; init; }
}