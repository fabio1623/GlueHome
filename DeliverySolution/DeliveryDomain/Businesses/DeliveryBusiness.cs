using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.DomainModels.RabbitMqMessages;
using DeliveryDomain.Exceptions;
using DeliveryDomain.Interfaces.Businesses;
using DeliveryDomain.Interfaces.Services;

namespace DeliveryDomain.Businesses;

public class DeliveryBusiness : IDeliveryBusiness
{
    private readonly IDeliveryService _deliveryService;
    private readonly IRabbitMqService _rabbitMqService;

    public DeliveryBusiness(IDeliveryService deliveryService, IRabbitMqService rabbitMqService)
    {
        _deliveryService = deliveryService;
        _rabbitMqService = rabbitMqService;
    }

    public async Task Create(CreateDeliveryDomain? deliveryDomain, CancellationToken cancellationToken)
    {
        await _deliveryService.Create(deliveryDomain, cancellationToken);
        await ProduceDeliveryCreated(deliveryDomain?.Order?.OrderNumber);
    }

    public async Task<DomainModels.Deliveries.DeliveryDomain?> Get(string? orderNumber, CancellationToken cancellationToken)
    {
        return await _deliveryService.Get(orderNumber, cancellationToken);
    }

    public async Task Approve(string? orderNumber, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(orderNumber, cancellationToken);
        if (delivery?.State != StateDomain.Created)
            throw new DomainException($"Order '{orderNumber}' cannot be approved because it is not in '{nameof(StateDomain.Created)}' state. Current state : {delivery?.State}.");
        
        var deliveryUpdateDomain = new DeliveryUpdateDomain
        {
            State = StateDomain.Approved
        };
        await _deliveryService.Update(orderNumber, deliveryUpdateDomain, cancellationToken);
        await ProduceDeliveryUpdated(orderNumber, deliveryUpdateDomain.State);
    }

    public async Task Complete(string? orderNumber, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(orderNumber, cancellationToken);
        if (delivery?.State != StateDomain.Approved)
            throw new DomainException($"Order '{orderNumber}' cannot be completed because it is not in '{nameof(StateDomain.Approved)}' state. Current state : {delivery?.State}.");
        
        var deliveryUpdateDomain = new DeliveryUpdateDomain
        {
            State = StateDomain.Completed
        };
        await _deliveryService.Update(orderNumber, deliveryUpdateDomain, cancellationToken);
        await ProduceDeliveryUpdated(orderNumber, deliveryUpdateDomain.State);
    }

    public async Task Cancel(string? orderNumber, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(orderNumber, cancellationToken);
        if (delivery?.State != StateDomain.Created && delivery?.State != StateDomain.Approved)
            throw new DomainException($"Order '{orderNumber}' cannot be cancelled because it is not in '{nameof(StateDomain.Created)}' or '{nameof(StateDomain.Approved)}' state. Current state : {delivery?.State}.");
        
        var deliveryUpdateDomain = new DeliveryUpdateDomain
        {
            State = StateDomain.Cancelled
        };
        await _deliveryService.Update(orderNumber, deliveryUpdateDomain, cancellationToken);
        await ProduceDeliveryUpdated(orderNumber, deliveryUpdateDomain.State);
    }

    public async Task Delete(string? orderNumber, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(orderNumber, cancellationToken);
        if (delivery == null)
            throw new DomainException($"Order '{orderNumber}' does not exist.");
        
        await _deliveryService.Delete(orderNumber, cancellationToken);
        await ProduceDeliveryDeleted(orderNumber);
    }
    
    private async Task ProduceDeliveryCreated(string? orderNumber)
    {
        var deliveryCreatedMessage = new DeliveryCreatedMessage
        {
            OrderNumber = orderNumber
        };
        await _rabbitMqService.ProduceMessage(deliveryCreatedMessage);
    }
    
    private async Task ProduceDeliveryUpdated(string? orderNumber, StateDomain? newState)
    {
        var deliveryUpdatedMessage = new DeliveryUpdatedMessage
        {
            OrderNumber = orderNumber,
            NewState = newState?.ToString()
        };
        await _rabbitMqService.ProduceMessage(deliveryUpdatedMessage);
    }
    
    private async Task ProduceDeliveryDeleted(string? orderNumber)
    {
        var deliveryUpdatedMessage = new DeliveryDeletedMessage
        {
            OrderNumber = orderNumber
        };
        await _rabbitMqService.ProduceMessage(deliveryUpdatedMessage);
    }
}