using DeliveryDomain.DomainModels.Users;

namespace DeliveryApi.Models.Users;

public class AuthenticateResponse
{
    public User? User { get; set; }
    public string? Token { get; set; }

    public AuthenticateResponse(AuthenticateResponseDomain? authenticateResponseDomain)
    {
        User = new User(authenticateResponseDomain?.UserDomain);
        Token = authenticateResponseDomain?.Token;
    }
}