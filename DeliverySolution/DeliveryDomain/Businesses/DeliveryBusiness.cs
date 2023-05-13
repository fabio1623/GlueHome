using DeliveryDomain.DomainEnums;
using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.BrokerMessages;
using DeliveryDomain.DomainModels.Deliveries;
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

    public async Task<DomainModels.DeliveryDomain?> Create(CreateDeliveryRequestDomain? deliveryDomain, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Create(deliveryDomain, cancellationToken);
        await ProduceDeliveryCreated(delivery?.Id);
        return delivery;
    }

    public async Task<PagedListDomain<DomainModels.DeliveryDomain?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken)
    {
        return await _deliveryService.GetPaged(requestedPage, pageSize, cancellationToken);
    }

    public async Task<DomainModels.DeliveryDomain?> Get(string? deliveryId, CancellationToken cancellationToken)
    {
        return await _deliveryService.Get(deliveryId, cancellationToken);
    }

    public async Task Approve(string? deliveryId, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(deliveryId, cancellationToken);
        if (delivery?.State != StateDomain.Created)
            throw new DomainException($"Order '{deliveryId}' cannot be approved because it is not in '{nameof(StateDomain.Created)}' state. Current state : {delivery?.State}.");
        
        await UpdateAndProduceDeliveryUpdated(deliveryId, StateDomain.Approved, cancellationToken);
    }

    public async Task Complete(string? deliveryId, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(deliveryId, cancellationToken);
        if (delivery?.State != StateDomain.Approved)
            throw new DomainException($"Order '{deliveryId}' cannot be completed because it is not in '{nameof(StateDomain.Approved)}' state. Current state : {delivery?.State}.");
        
        await UpdateAndProduceDeliveryUpdated(deliveryId, StateDomain.Completed, cancellationToken);
    }

    public async Task Cancel(string? deliveryId, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(deliveryId, cancellationToken);
        if (delivery?.State != StateDomain.Created && delivery?.State != StateDomain.Approved)
            throw new DomainException($"Order '{deliveryId}' cannot be cancelled because it is not in '{nameof(StateDomain.Created)}' or '{nameof(StateDomain.Approved)}' state. Current state : {delivery?.State}.");
        
        await UpdateAndProduceDeliveryUpdated(deliveryId, StateDomain.Cancelled, cancellationToken);
    }

    public async Task Delete(string? deliveryId, CancellationToken cancellationToken)
    {
        var delivery = await _deliveryService.Get(deliveryId, cancellationToken);
        if (delivery == null)
            throw new DomainException($"Order '{deliveryId}' does not exist.");
        
        await _deliveryService.Delete(deliveryId, cancellationToken);
        await ProduceDeliveryDeleted(deliveryId);
    }
    
    private async Task ProduceDeliveryCreated(string? deliveryId)
    {
        var deliveryCreatedMessage = new DeliveryCreatedMessage
        {
            DeliveryId = deliveryId
        };
        await _rabbitMqService.ProduceMessage(deliveryCreatedMessage);
    }
    
    private async Task UpdateAndProduceDeliveryUpdated(string? deliveryId, StateDomain state, CancellationToken cancellationToken)
    {
        await UpdateDelivery(deliveryId, state, cancellationToken);
        await ProduceDeliveryUpdated(deliveryId, state);
    }

    private async Task UpdateDelivery(string? deliveryId, StateDomain state, CancellationToken cancellationToken)
    {
        var deliveryUpdateDomain = new UpdateDeliveryRequestDomain
        {
            State = state
        };
        await _deliveryService.Update(deliveryId, deliveryUpdateDomain, cancellationToken);
    }

    private async Task ProduceDeliveryUpdated(string? deliveryId, StateDomain? newState)
    {
        var deliveryUpdatedMessage = new DeliveryUpdatedMessage
        {
            DeliveryId = deliveryId,
            NewState = newState?.ToString()
        };
        await _rabbitMqService.ProduceMessage(deliveryUpdatedMessage);
    }
    
    private async Task ProduceDeliveryDeleted(string? deliveryId)
    {
        var deliveryUpdatedMessage = new DeliveryDeletedMessage
        {
            DeliveryId = deliveryId
        };
        await _rabbitMqService.ProduceMessage(deliveryUpdatedMessage);
    }
}