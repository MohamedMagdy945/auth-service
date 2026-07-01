using auth.Application.Common;
using auth.Application.Features.Auth.Login;
using auth.Application.Interfaces;
using auth.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace auth.Application.Tests.Features.Auth.Login;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_returns_unauthorized_for_invalid_password()
    {
        var options = new DbContextOptionsBuilder<TestAuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestAuthDbContext(options);
        context.Users.Add(new User
        {
            Email = "user@gmail.com",
            FullName = "Test User",
            IsEnabled = true,
            PasswordHash = "hashed"
        });
        await context.SaveChangesAsync();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher
            .Setup(h => h.Verify("WrongPassword123", "hashed"))
            .Returns(false);

        var handler = new LoginCommandHandler(
            context,
            passwordHasher.Object,
            Mock.Of<ITokenGenerator>(),
            Mock.Of<IClientInfoProvider>(),
            NullLogger<LoginCommandHandler>.Instance);

        var result = await handler.Handle(
            new LoginCommand("user@gmail.com", "WrongPassword123"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Handle_returns_unauthorized_for_disabled_user()
    {
        var options = new DbContextOptionsBuilder<TestAuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestAuthDbContext(options);
        context.Users.Add(new User
        {
            Email = "user@gmail.com",
            FullName = "Test User",
            IsEnabled = false,
            PasswordHash = "hashed"
        });
        await context.SaveChangesAsync();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher
            .Setup(h => h.Verify("Password123", "hashed"))
            .Returns(true);

        var handler = new LoginCommandHandler(
            context,
            passwordHasher.Object,
            Mock.Of<ITokenGenerator>(),
            Mock.Of<IClientInfoProvider>(),
            NullLogger<LoginCommandHandler>.Instance);

        var result = await handler.Handle(
            new LoginCommand("user@gmail.com", "Password123"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Handle_returns_tokens_for_valid_login()
    {
        var options = new DbContextOptionsBuilder<TestAuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestAuthDbContext(options);
        var user = new User
        {
            Email = "user@gmail.com",
            FullName = "Test User",
            IsEnabled = true,
            PasswordHash = "hashed"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher
            .Setup(h => h.Verify("Password123", "hashed"))
            .Returns(true);

        var tokenGenerator = new Mock<ITokenGenerator>();
        tokenGenerator
            .Setup(g => g.GenerateTokenPair(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenPairResponse
            {
                UserId = user.Id,
                Email = user.Email,
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(30),
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
            });
        tokenGenerator
            .Setup(g => g.HashToken("refresh-token"))
            .Returns("hashed-refresh-token");

        var clientInfo = new Mock<IClientInfoProvider>();
        clientInfo.Setup(c => c.GetIpAddress()).Returns("127.0.0.1");
        clientInfo.Setup(c => c.GetUserAgent()).Returns("test-agent");

        var handler = new LoginCommandHandler(
            context,
            passwordHasher.Object,
            tokenGenerator.Object,
            clientInfo.Object,
            NullLogger<LoginCommandHandler>.Instance);

        var result = await handler.Handle(
            new LoginCommand("user@gmail.com", "Password123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("access-token", result.Data!.AccessToken);
        Assert.Single(context.RefreshTokens);
    }
}
