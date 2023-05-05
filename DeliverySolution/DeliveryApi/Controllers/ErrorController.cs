using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerExtensions
{
    [Route("/errors")]
    public IActionResult HandleErrors()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return context?.Error switch
        {
            OperationCanceledException operationCanceledException => BadRequestProblem(operationCanceledException),
            _ => Problem(context?.Error)
        };
    }
}