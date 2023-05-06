using DeliveryDomain.DomainModels.Deliveries;
using FluentValidation;

namespace DeliveryApi.Models.Deliveries;

public class CreateDelivery
{
    public DateTime? EndTime { get; set; }
    public Recipient? Recipient { get; set; }
    public Order? Order { get; set; }
    
    public CreateDeliveryDomain ToDomain()
    {
        return new CreateDeliveryDomain
        {
            EndTime = EndTime,
            Recipient = Recipient?.ToDomain(),
            Order = Order?.ToDomain()
        };
    }
}

public class CreateDeliveryValidator : AbstractValidator<CreateDelivery?>
{
    public CreateDeliveryValidator()
    {
        RuleFor(x => x!.EndTime)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x!.Recipient)
            .SetValidator(new RecipientValidation());
        
        RuleFor(x => x!.Order)
            .SetValidator(new OrderValidator());
    }
}