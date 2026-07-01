using auth.Application.Features.Auth.Login;
using FluentValidation.TestHelper;

namespace auth.Application.Tests.Features.Auth.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_fail_when_email_is_empty()
    {
        var command = new LoginCommand(string.Empty, "Password123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_fail_when_password_is_too_short()
    {
        var command = new LoginCommand("user@gmail.com", "short");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_fail_when_password_exceeds_max_length()
    {
        var command = new LoginCommand("user@gmail.com", new string('a', 129));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_fail_when_email_has_whitespace_or_uppercase()
    {
        var command = new LoginCommand(" User@gmail.com ", "Password123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_pass_for_valid_credentials()
    {
        var command = new LoginCommand("user@gmail.com", "Password123");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
