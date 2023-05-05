using DeliveryApi.Attributes;
using DeliveryApi.Models.Users;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApi.Controllers;

[Authorize(Role.Admin)]
public class UsersController : ControllerExtensions
{
    private readonly IUserBusiness _userBusiness;

    public UsersController(IUserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var authenticateRequestDomain = model.ToDomainModel();
        var authenticateResponseDomain = _userBusiness.Authenticate(authenticateRequestDomain);
        
        return authenticateResponseDomain == null
            ? Problem("Could not authenticate user.")
            : new AuthenticateResponse(authenticateResponseDomain);
    }

    [HttpGet]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public ActionResult<IEnumerable<User>?> GetAll()
    {
        var userDomainList = _userBusiness.GetAll();
        var users = userDomainList?.Select(x => new User(x));
        return users?.ToList();
    }

    [HttpGet("{userId:int}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public ActionResult<User> Get(int userId)
    {
        var userDomain = _userBusiness.GetById(userId);
        
        return userDomain == null ?
            NotFoundProblem($"User '{userId}' does not exist.", type : nameof(DeliveriesController)) :
            new User(userDomain);
    }
}