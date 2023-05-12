using FluentValidation;

namespace DeliveryApi.Models.Users;

public class AuthenticateUserRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class AuthenticateUserRequestValidator : AbstractValidator<AuthenticateUserRequest>
{
    public AuthenticateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();
        
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}