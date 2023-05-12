namespace DeliveryDomain.DomainModels.Users;

public class AuthenticateUserRequestDomain
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}