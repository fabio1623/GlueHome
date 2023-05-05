namespace DeliveryDomain.DomainModels.Users;

public class AuthenticateResponseDomain
{
    public UserDomain? UserDomain { get; set; }
    public string? Token { get; set; }

    public AuthenticateResponseDomain(UserDomain? user, string? token)
    {
        UserDomain = user;
        Token = token;
    }
}