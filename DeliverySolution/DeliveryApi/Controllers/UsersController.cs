using DeliveryApi.Attributes;
using DeliveryApi.Models.Users;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApi.Controllers;

[Authorize(Role.Admin)]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserBusiness _userBusiness;

    public UsersController(IUserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var authenticateRequestDomain = model.ToDomainModel();
        var authenticateResponseDomain = _userBusiness.Authenticate(authenticateRequestDomain);
        return Ok(new AuthenticateResponse(authenticateResponseDomain));
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var userDomainList = _userBusiness.GetAll();
        var users = userDomainList.Select(x => new User(x));
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var userDomain = _userBusiness.GetById(id);
        return Ok(new User(userDomain));
        // // only admins can access other user records
        // var currentUser = (UserInfra?)HttpContext.Items["User"];
        // if (id != currentUser?.Id && currentUser?.RoleInfra != RoleInfra.Admin)
        //     return Unauthorized(new { message = "Unauthorized" });
        //
        // var user =  _userService.GetById(id);
        // return Ok(user);
    }
}