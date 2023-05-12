namespace DeliveryApi.Models.Users;

public class AuthenticateUserResponse
{
    public User? User { get; set; }
    public string? Token { get; set; }
}