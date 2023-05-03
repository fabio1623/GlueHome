using Delivery.API.Entities;

namespace Delivery.API.Authorization;

public interface IJwtUtils
{
    public string GenerateJwtToken(User user);
    public int? ValidateJwtToken(string token);
}