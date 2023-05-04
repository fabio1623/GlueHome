using DeliveryDomain.DomainModels;
using DeliveryDomain.Exceptions;
using DeliveryDomain.Interfaces.Businesses;
using DeliveryDomain.Interfaces.Services;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DeliveryDomain.Businesses;

public class UserBusiness : IUserBusiness
{
    private readonly IUserService _userService;
    private readonly IJwtUtils _jwtUtils;

    public UserBusiness(IUserService userService, IJwtUtils jwtUtils)
    {
        _userService = userService;
        _jwtUtils = jwtUtils;
    }
    
    public AuthenticateResponseDomain Authenticate(AuthenticateRequestDomain authenticateRequestDomain)
    {
        var user = _userService.GetByUsername(authenticateRequestDomain.Username);
        
        // validate
        if (user == null || !BCryptNet.Verify(authenticateRequestDomain.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        // authentication successful so generate jwt token
        var jwtToken = _jwtUtils.GenerateJwtToken(user);

        return new AuthenticateResponseDomain(user, jwtToken);
    }

    public IEnumerable<UserDomain>? GetAll()
    {
        return _userService.GetAll();
    }

    public UserDomain GetById(int id)
    {
        var user = _userService.GetById(id);
        if (user == null) 
            throw new KeyNotFoundException("User not found");
        
        return user;
    }
}