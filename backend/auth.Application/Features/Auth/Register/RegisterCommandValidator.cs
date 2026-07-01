using FluentValidation;

namespace auth.Application.Features.Auth.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .Must(e => e == e.Trim().ToLowerInvariant())
            .WithMessage("Email must be lowercase with no surrounding whitespace");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.Password)
          .NotEmpty()
          .WithMessage("Password is required.")
          .MinimumLength(8)
          .MaximumLength(128)
          .WithMessage("Password must be between 8 and 128 characters.");

        RuleFor(x => x.ConfirmPassword)
          .NotEmpty()
          .WithMessage("Confirm password is required.")
          .Equal(x => x.Password)
          .WithMessage("Passwords do not match.");
    }
}