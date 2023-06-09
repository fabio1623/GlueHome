using AutoMapper;
using DeliveryApi.Attributes;
using DeliveryApi.Enums;
using DeliveryApi.Models;
using DeliveryApi.Models.Users;
using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApi.Controllers;

[Authorize(Role.Admin)]
public class UsersController : ControllerExtensions
{
    private readonly IUserBusiness _userBusiness;
    private readonly IMapper _mapper;

    public UsersController(IUserBusiness userBusiness, IMapper mapper)
    {
        _userBusiness = userBusiness;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<ActionResult<AuthenticateUserResponse?>> Authenticate(AuthenticateUserRequest? request, CancellationToken cancellationToken)
    {
        var requestDomain = _mapper.Map<AuthenticateUserDomain>(request);
        var responseDomain = await _userBusiness.Authenticate(requestDomain, cancellationToken);
        
        return responseDomain == null
            ? Problem("Could not authenticate user.")
            : _mapper.Map<AuthenticateUserResponse?>(responseDomain);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<PagedList<User>?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken)
    {
        var userDomainList = await _userBusiness.GetPaged(requestedPage, pageSize, cancellationToken);
        return _mapper.Map<PagedList<User>?>(userDomainList);
    }

    [HttpGet("{userId}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<User?>> Get(string? userId, CancellationToken cancellationToken)
    {
        var userDomain = await _userBusiness.Get(userId, cancellationToken);
        
        return userDomain == null ?
            NotFoundProblem($"User '{userId}' does not exist.", type : nameof(DeliveriesController)) :
            _mapper.Map<User?>(userDomain);
    }
}