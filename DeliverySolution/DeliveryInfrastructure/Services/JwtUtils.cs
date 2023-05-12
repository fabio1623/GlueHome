using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeliveryDomain.DomainModels;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DeliveryInfrastructure.Services;

public class JwtUtils : IJwtUtils
{
    private const string UserIdClaimType = "userId";

    private readonly ILogger<JwtUtils> _logger;
    private readonly SymmetricSecurityKey? _symmetricSecurityKey;

    public JwtUtils(IAppSettings appSettings, ILogger<JwtUtils> logger)
    {
        _logger = logger;
        
        if (string.IsNullOrWhiteSpace(appSettings.Secret))
        {
            _logger.LogCritical($"'{nameof(appSettings.Secret)}' is not set.");
            return;
        }
        
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);
        _symmetricSecurityKey = new SymmetricSecurityKey(key);
    }

    public string? GenerateJwtToken(UserDomain? user)
    {
        var userId = user?.Id;
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning($"'{nameof(user.Id)} from {nameof(UserDomain)}' is not set.");
            return string.Empty;
        }

        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(UserIdClaimType, userId) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateJwtToken(string? token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _symmetricSecurityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == UserIdClaimType)?.Value;

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}