using DeliveryDomain.DomainModels.Deliveries;
using FluentValidation;

namespace DeliveryApi.Models.Deliveries;

public class Order
{
    public string? OrderNumber { get; set; }
    public string? Sender { get; set; }

    public Order()
    {
    }
    
    public Order(OrderDomain? orderDomain)
    {
        OrderNumber = orderDomain?.OrderNumber;
        Sender = orderDomain?.Sender;
    }

    public OrderDomain ToDomain()
    {
        return new OrderDomain
        {
            OrderNumber = OrderNumber,
            Sender = Sender
        };
    }
}

public class OrderValidator : AbstractValidator<Order?>
{
    public OrderValidator()
    {
        RuleFor(x => x!.OrderNumber)
            .NotEmpty()
            .Must(x => x != null && !x.Contains(' '))
            .WithMessage($"Spaces are not allowed in {nameof(Order.OrderNumber)}.");

        RuleFor(x => x!.Sender)
            .NotEmpty();
    }
}