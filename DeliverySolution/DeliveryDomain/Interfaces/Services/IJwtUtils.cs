using DeliveryDomain.DomainModels;

namespace DeliveryDomain.Interfaces.Services;

public interface IJwtUtils
{
    public string? GenerateJwtToken(UserDomain user);
    public string? ValidateJwtToken(string? token);
}