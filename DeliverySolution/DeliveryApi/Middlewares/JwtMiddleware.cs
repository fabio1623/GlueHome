using AutoMapper;
using DeliveryApi.Models;
using DeliveryDomain.Interfaces.Services;

namespace DeliveryApi.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMapper _mapper;

    public JwtMiddleware(RequestDelegate next, IMapper mapper)
    {
        _next = next;
        _mapper = mapper;
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
                var userDomain = await userService.Get(userId, CancellationToken.None);
                if (userDomain != null)
                    context.Items["User"] = _mapper.Map<User>(userDomain);
            }
        }

        await _next(context);
    }
}