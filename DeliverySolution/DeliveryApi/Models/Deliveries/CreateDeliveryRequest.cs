using FluentValidation;

namespace DeliveryApi.Models.Deliveries;

public class CreateDeliveryRequest
{
    public DateTime? EndTime { get; set; }
    public CreateDeliveryRecipient? Recipient { get; set; }
    public CreateDeliveryOrder? Order { get; set; }

    public class CreateDeliveryRecipient
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class CreateDeliveryOrder
    {
        public string? OrderNumber { get; set; }
        public string? Sender { get; set; }
    }
}

public class CreateDeliveryRequestValidator : AbstractValidator<CreateDeliveryRequest?>
{
    public CreateDeliveryRequestValidator()
    {
        RuleFor(x => x!.EndTime)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x!.Recipient)
            .SetValidator(new CreateDeliveryRecipientValidation());
        
        RuleFor(x => x!.Order)
            .SetValidator(new CreateDeliveryOrderValidator());
    }
}

public class CreateDeliveryRecipientValidation : AbstractValidator<CreateDeliveryRequest.CreateDeliveryRecipient?>
{
    public CreateDeliveryRecipientValidation()
    {
        RuleFor(x => x!.Name)
            .NotEmpty();
        
        RuleFor(x => x!.Address)
            .NotEmpty();
        
        RuleFor(x => x!.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x!.Email)
            .NotEmpty();
    }
}

public class CreateDeliveryOrderValidator : AbstractValidator<CreateDeliveryRequest.CreateDeliveryOrder?>
{
    public CreateDeliveryOrderValidator()
    {
        RuleFor(x => x!.OrderNumber)
            .NotEmpty()
            .Must(x => x != null && !x.Contains(' '))
            .WithMessage($"Spaces are not allowed in {nameof(CreateDeliveryRequest.CreateDeliveryOrder.OrderNumber)}.");

        RuleFor(x => x!.Sender)
            .NotEmpty();
    }
}