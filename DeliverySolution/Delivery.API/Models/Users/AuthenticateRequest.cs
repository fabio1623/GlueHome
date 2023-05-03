using System.ComponentModel.DataAnnotations;

namespace Delivery.API.Models.Users;

public class AuthenticateRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}