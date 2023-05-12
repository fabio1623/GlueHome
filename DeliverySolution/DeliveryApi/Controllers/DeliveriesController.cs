using AutoMapper;
using DeliveryApi.Attributes;
using DeliveryApi.Enums;
using DeliveryApi.Models;
using DeliveryApi.Models.Deliveries;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DeliveryApi.Controllers;

[Authorize]
public class DeliveriesController : ControllerExtensions
{
    private readonly IDeliveryBusiness _deliveryBusiness;
    private readonly IMapper _mapper;

    public DeliveriesController(IDeliveryBusiness deliveryBusiness, IMapper mapper)
    {
        _deliveryBusiness = deliveryBusiness;
        _mapper = mapper;
    }
    
    [Authorize(Role.User, Role.Partner)]
    [HttpPost("[action]")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
    public async Task<IActionResult> Create(CreateDeliveryRequest? request, CancellationToken cancellationToken)
    {
        var requestDomain = _mapper.Map<CreateDeliveryRequestDomain>(request);
        await _deliveryBusiness.Create(requestDomain, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(Role.Admin)]
    [HttpGet]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<PagedList<Delivery?>>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken)
    {
        var validationResult = await ValidatePagination(
            ("Page Size", pageSize),
            ("Requested Page", requestedPage)
        );
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        var pagedItems = await _deliveryBusiness.GetPaged(requestedPage, pageSize, cancellationToken);
        return _mapper.Map<PagedList<Delivery?>>(pagedItems);
    }
    
    [Authorize(Role.User, Role.Partner)]
    [HttpGet("{deliveryId}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<Delivery?>> Get(string? deliveryId, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(deliveryId)}", deliveryId);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);
        
        var domainModel = await _deliveryBusiness.Get(deliveryId, cancellationToken);
        
        return domainModel == null ?
            NotFoundProblem($"Delivery '{deliveryId}' does not exist.", type : nameof(DeliveriesController)) :
            _mapper.Map<Delivery?>(domainModel);
    }

    [Authorize(Role.User)]
    [HttpPut("[action]/{deliveryId}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> Approve(string? deliveryId, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(deliveryId)}", deliveryId);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Approve(deliveryId, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(Role.Partner)]
    [HttpPut("[action]/{deliveryId}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> Complete(string? deliveryId, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(deliveryId)}", deliveryId);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Complete(deliveryId, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(Role.User, Role.Partner)]
    [HttpPut("[action]/{deliveryId}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
    public async Task<IActionResult> Cancel(string? deliveryId, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(deliveryId)}", deliveryId);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Cancel(deliveryId, cancellationToken);
        
        return NoContent();
    }
    
    [Authorize(Role.Admin)]
    [HttpDelete("{deliveryId}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<IActionResult> Delete(string? deliveryId, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateOrderNumber($"{nameof(deliveryId)}", deliveryId);
        if (!validationResult.IsValid)
            return ValidationProblem(validationResult);

        await _deliveryBusiness.Delete(deliveryId, cancellationToken);
        
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