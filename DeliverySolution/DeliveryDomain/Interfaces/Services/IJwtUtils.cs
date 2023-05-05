using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Services;

public interface IJwtUtils
{
    public string? GenerateJwtToken(UserDomain user);
    public int? ValidateJwtToken(string? token);
}