using auth.Application.Common;
using auth.Application.DTOs;
using auth.Application.Features.Auth.Login;
using auth.Application.Interfaces;
using Auth.Application.Bases;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace auth.Application.Tests.Features.Auth.Login;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_enriches_request_with_client_info_and_delegates_to_auth_service()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TokenPairResponse>.Success(new TokenPairResponse
            {
                UserId = 1,
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            }));

        var clientInfo = new Mock<IClientInfoProvider>();
        clientInfo.Setup(c => c.GetIpAddress()).Returns("10.0.0.1");
        clientInfo.Setup(c => c.GetUserAgent()).Returns("Mozilla/5.0");

        var handler = new LoginCommandHandler(
            authService.Object,
            clientInfo.Object,
            NullLogger<LoginCommandHandler>.Instance);

        var result = await handler.Handle(
            new LoginCommand("user@gmail.com", "Password123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        authService.Verify(s => s.LoginAsync(
            It.Is<LoginRequest>(r =>
                r.Email == "user@gmail.com" &&
                r.Password == "Password123" &&
                r.IpAddress == "10.0.0.1" &&
                r.DeviceInfo == "Mozilla/5.0"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_returns_failure_from_auth_service()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<TokenPairResponse>.Unauthorized("Invalid email or password."));

        var handler = new LoginCommandHandler(
            authService.Object,
            Mock.Of<IClientInfoProvider>(),
            NullLogger<LoginCommandHandler>.Instance);

        var result = await handler.Handle(
            new LoginCommand("user@gmail.com", "wrong"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(401, result.StatusCode);
    }
}
