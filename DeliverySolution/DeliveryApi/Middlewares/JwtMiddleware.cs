using DeliveryApi.Models.Users;
using DeliveryDomain.Interfaces.Services;

namespace DeliveryApi.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").LastOrDefault();
        if (token != null)
        {
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                var userDomain = userService.GetById(userId.Value);
                if (userDomain != null)
                    context.Items["User"] = new User(userDomain);
            }
        }

        await _next(context);
    }
}