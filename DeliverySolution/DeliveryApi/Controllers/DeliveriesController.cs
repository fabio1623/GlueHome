using DeliveryApi.Attributes;
using DeliveryApi.Models.Deliveries;
using DeliveryApi.Models.Users;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliveryApi.Controllers;

[Authorize(Role.Partner, Role.User)]
public class DeliveriesController : ControllerExtensions
{
    private readonly IDeliveryBusiness _deliveryBusiness;

    public DeliveriesController(IDeliveryBusiness deliveryBusiness)
    {
        _deliveryBusiness = deliveryBusiness;
    }
    
    [HttpPost("[action]")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<IActionResult> Create(CreateDelivery createDelivery, CancellationToken cancellationToken)
    {
        var createModel = createDelivery.ToDomain();
        await _deliveryBusiness.Create(createModel, cancellationToken);
        
        return NoContent();
    }
    
    [HttpGet("{orderNumber}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<Delivery>> Get(string orderNumber, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(orderNumber)}", orderNumber);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);
        
        var domainModel = await _deliveryBusiness.Get(orderNumber, cancellationToken);
        
        return domainModel == null ?
            NotFoundProblem($"Delivery '{orderNumber}' does not exist.", type : nameof(DeliveriesController)) :
            new Delivery(domainModel);
    }

    [Authorize(Role.User)]
    [HttpPut("[action]/{orderNumber}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> Approve(string? orderNumber, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(orderNumber)}", orderNumber);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Approve(orderNumber, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(Role.Partner)]
    [HttpPut("[action]/{orderNumber}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> Complete(string? orderNumber, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(orderNumber)}", orderNumber);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Complete(orderNumber, cancellationToken);
        
        return NoContent();
    }
    
    [HttpPut("[action]/{orderNumber}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> Cancel(string? orderNumber, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(orderNumber)}", orderNumber);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Cancel(orderNumber, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(Role.Admin)]
    [HttpDelete("{orderNumber}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<IActionResult> Delete(string? orderNumber, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(orderNumber)}", orderNumber);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Delete(orderNumber, cancellationToken);
        
        return NoContent();
    }
    
    private static Task<ModelStateDictionary> ValidateOrderNumber(string? fieldName, string? fieldValue)
    {
        var modelStateDictionary = new ModelStateDictionary();
        if (!string.IsNullOrWhiteSpace(fieldName) && string.IsNullOrWhiteSpace(fieldValue))
            modelStateDictionary.AddModelError(fieldName, $"'{fieldName}' has to be provided.");
        
        return Task.FromResult(modelStateDictionary);
    }
}