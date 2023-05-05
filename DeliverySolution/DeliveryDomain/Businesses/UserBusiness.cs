using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Businesses;
using DeliveryDomain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DeliveryDomain.Businesses;

public class UserBusiness : IUserBusiness
{
    private readonly IUserService _userService;
    private readonly IJwtUtils _jwtUtils;
    private readonly ILogger<UserBusiness> _logger;

    public UserBusiness(IUserService userService, IJwtUtils jwtUtils, ILogger<UserBusiness> logger)
    {
        _userService = userService;
        _jwtUtils = jwtUtils;
        _logger = logger;
    }
    
    public AuthenticateResponseDomain? Authenticate(AuthenticateRequestDomain? authenticateRequestDomain)
    {
        var user = _userService.GetByUsername(authenticateRequestDomain?.Username);
        
        // validate
        if (user == null || !BCryptNet.Verify(authenticateRequestDomain?.Password, user.PasswordHash))
        {
            _logger.LogWarning("Username or password is incorrect.");
            return null;   
        }

        // authentication successful so generate jwt token
        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        if (!string.IsNullOrWhiteSpace(jwtToken)) 
            return new AuthenticateResponseDomain(user, jwtToken);
        
        _logger.LogWarning("Could not generate Jwt Token.");
        return null;
    }

    public IEnumerable<UserDomain>? GetAll()
    {
        return _userService.GetAll();
    }

    public UserDomain? GetById(int? userId)
    {
        return _userService.GetById(userId);
    }
}