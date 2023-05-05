namespace DeliveryDomain.DomainModels.Users;

public class AuthenticateRequestDomain
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}