using DeliveryDomain.DomainModels;
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
    
    public async Task<AuthenticateUserResponseDomain?> Authenticate(AuthenticateUserRequestDomain? authenticateRequestDomain, CancellationToken cancellationToken)
    {
        var userDomain = await _userService.GetByUsername(authenticateRequestDomain?.Username, cancellationToken);
        
        // validate
        if (userDomain == null || !BCryptNet.Verify(authenticateRequestDomain?.Password, userDomain.PasswordHash))
        {
            _logger.LogWarning("Username or password is incorrect.");
            return null;   
        }

        // authentication successful so generate jwt token
        var jwtToken = _jwtUtils.GenerateJwtToken(userDomain);
        if (!string.IsNullOrWhiteSpace(jwtToken))
            return new AuthenticateUserResponseDomain
            {
                User = userDomain,
                Token = jwtToken
            };
        
        _logger.LogWarning("Could not generate Jwt Token.");
        return null;
    }

    public async Task<PagedListDomain<UserDomain?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken)
    {
        return await _userService.GetPaged(requestedPage, pageSize, cancellationToken);
    }

    public async Task<UserDomain?> Get(string? id, CancellationToken cancellationToken)
    {
        return await _userService.Get(id, cancellationToken);
    }
}