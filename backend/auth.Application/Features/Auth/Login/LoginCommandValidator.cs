using FluentValidation;

namespace auth.Application.Features.Auth.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .Must(e => e == e.Trim().ToLowerInvariant())
            .WithMessage("Email must be lowercase with no surrounding whitespace");

        RuleFor(x => x.Password)
           .NotEmpty()
           .WithMessage("Password is required.")
           .MinimumLength(8)
           .MaximumLength(128)
           .WithMessage("Password must be between 8 and 128 characters.")
           .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).+$")
           .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
    }
}
