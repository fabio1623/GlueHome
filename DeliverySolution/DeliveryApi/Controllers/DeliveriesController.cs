using DeliveryApi.Attributes;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryBusiness _deliveryBusiness;

    public DeliveriesController(IDeliveryBusiness deliveryBusiness)
    {
        _deliveryBusiness = deliveryBusiness;
    }
}