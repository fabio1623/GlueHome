using Microsoft.AspNetCore.Mvc;

namespace DeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class ControllerExtensions : ControllerBase
{
    protected ActionResult NotFoundProblem(string? message, string? type)
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: message,
            type : type);
    }
    
    protected ActionResult BadRequestProblem(Exception? exception)
    {
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: exception?.Message,
            type: exception?.GetType().Name);
    }

    protected ActionResult Problem(Exception? exception)
    {
        return Problem(
            title: exception?.Message,
            detail: exception?.StackTrace);
    }
}