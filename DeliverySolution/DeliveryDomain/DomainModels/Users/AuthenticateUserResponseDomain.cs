namespace DeliveryDomain.DomainModels.Users;

public class AuthenticateUserResponseDomain
{
    public UserDomain? User { get; set; }
    public string? Token { get; set; }
}