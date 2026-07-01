using auth.Application.DTOs;
using auth.Application.Features.Auth.Logout;
using FluentValidation.TestHelper;

namespace auth.Application.Tests.Features.Auth.Logout;

public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Fact]
    public void Should_fail_when_refresh_token_is_empty()
    {
        var command = new LogoutCommand(string.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Fact]
    public void Should_pass_when_refresh_token_is_provided()
    {
        var command = new LogoutCommand("valid-refresh-token");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
