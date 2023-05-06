using DeliveryDomain.DomainModels.Deliveries;
using FluentValidation;

namespace DeliveryApi.Models.Deliveries;

public class Recipient
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public Recipient()
    {
    }

    public Recipient(RecipientDomain? recipientDomain)
    {
        Name = recipientDomain?.Name;
        Address = recipientDomain?.Address;
        Email = recipientDomain?.Email;
        PhoneNumber = recipientDomain?.PhoneNumber;
    }

    public RecipientDomain ToDomain()
    {
        return new RecipientDomain
        {
            Name = Name,
            Address = Address,
            Email = Email,
            PhoneNumber = PhoneNumber
        };
    }
}

public class RecipientValidation : AbstractValidator<Recipient?>
{
    public RecipientValidation()
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