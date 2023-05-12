using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
    
    protected static Task<ModelStateDictionary> ValidatePagination(params (string? fieldName, int? fieldValue)[] fields)
    {
        var modelStateDictionary = new ModelStateDictionary();
        foreach (var (fieldName, fieldValue) in fields)
            if (!string.IsNullOrWhiteSpace(fieldName) && fieldValue <= 0)
                modelStateDictionary.AddModelError(fieldName, $"'{fieldName}' must be greater than '0'.");

        return Task.FromResult(modelStateDictionary);
    }
}