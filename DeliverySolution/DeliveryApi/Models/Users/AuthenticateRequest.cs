using DeliveryDomain.DomainModels.Users;
using FluentValidation;

namespace DeliveryApi.Models.Users;

public class AuthenticateRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    
    public AuthenticateRequestDomain ToDomainModel()
    {
        return new AuthenticateRequestDomain
        {
            Username = Username,
            Password = Password
        };
    }
}

public class AuthenticateRequestValidator : AbstractValidator<AuthenticateRequest>
{
    public AuthenticateRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();
        
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}