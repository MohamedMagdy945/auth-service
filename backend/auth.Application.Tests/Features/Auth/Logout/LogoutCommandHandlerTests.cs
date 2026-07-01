using auth.Application.DTOs;
using auth.Application.Features.Auth.Logout;
using auth.Application.Interfaces;
using Auth.Application.Bases;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace auth.Application.Tests.Features.Auth.Logout;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_enriches_request_and_delegates_to_auth_service()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(s => s.LogoutAsync(It.IsAny<LogoutRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true, message: "Logged out successfully"));

        var clientInfo = new Mock<IClientInfoProvider>();
        clientInfo.Setup(c => c.GetIpAddress()).Returns("10.0.0.1");
        clientInfo.Setup(c => c.GetUserAgent()).Returns("Mozilla/5.0");

        var handler = new LogoutCommandHandler(
            authService.Object,
            clientInfo.Object,
            NullLogger<LogoutCommandHandler>.Instance);

        var result = await handler.Handle(
            new LogoutCommand("refresh-token"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        authService.Verify(s => s.LogoutAsync(
            It.Is<LogoutRequest>(r =>
                r.RefreshToken == "refresh-token" &&
                r.IpAddress == "10.0.0.1" &&
                r.DeviceInfo == "Mozilla/5.0"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_returns_failure_from_auth_service()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(s => s.LogoutAsync(It.IsAny<LogoutRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Unauthorized("Invalid refresh token."));

        var handler = new LogoutCommandHandler(
            authService.Object,
            Mock.Of<IClientInfoProvider>(),
            NullLogger<LogoutCommandHandler>.Instance);

        var result = await handler.Handle(
            new LogoutCommand("invalid-token"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(401, result.StatusCode);
    }
}
