using System.ComponentModel.DataAnnotations;
using DeliveryDomain.DomainModels;

namespace DeliveryApi.Models.Users;

public class AuthenticateRequest
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
    
    public AuthenticateRequestDomain ToDomainModel()
    {
        return new AuthenticateRequestDomain
        {
            Username = Username,
            Password = Password
        };
    }
}