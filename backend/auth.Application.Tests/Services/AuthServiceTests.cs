using auth.Application.Common;
using auth.Application.DTOs;
using auth.Application.Interfaces;
using auth.Application.Services;
using auth.Application.Tests;
using auth.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace auth.Application.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_returns_unauthorized_for_invalid_password()
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

        var service = new AuthService(
            context,
            passwordHasher.Object,
            Mock.Of<ITokenGenerator>());

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@gmail.com",
            Password = "WrongPassword123"
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_returns_unauthorized_for_disabled_user()
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

        var service = new AuthService(
            context,
            passwordHasher.Object,
            Mock.Of<ITokenGenerator>());

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@gmail.com",
            Password = "Password123"
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task LoginAsync_returns_tokens_for_valid_credentials()
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

        var service = new AuthService(
            context,
            passwordHasher.Object,
            tokenGenerator.Object);

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = "user@gmail.com",
            Password = "Password123",
            IpAddress = "127.0.0.1",
            DeviceInfo = "test-agent"
        });

        Assert.True(result.IsSuccess);
        Assert.Equal("access-token", result.Data!.AccessToken);
        Assert.Single(context.RefreshTokens);
    }

    [Fact]
    public async Task LogoutAsync_returns_unauthorized_for_invalid_token()
    {
        var options = new DbContextOptionsBuilder<TestAuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestAuthDbContext(options);

        var tokenGenerator = new Mock<ITokenGenerator>();
        tokenGenerator.Setup(g => g.HashToken("invalid-token")).Returns("invalid-hash");

        var service = new AuthService(
            context,
            Mock.Of<IPasswordHasher>(),
            tokenGenerator.Object);

        var result = await service.LogoutAsync(new LogoutRequest
        {
            RefreshToken = "invalid-token"
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task LogoutAsync_revokes_valid_refresh_token()
    {
        var options = new DbContextOptionsBuilder<TestAuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestAuthDbContext(options);
        context.RefreshTokens.Add(new RefreshToken
        {
            UserId = 1,
            TokenHash = "valid-hash",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });
        await context.SaveChangesAsync();

        var tokenGenerator = new Mock<ITokenGenerator>();
        tokenGenerator.Setup(g => g.HashToken("valid-token")).Returns("valid-hash");

        var service = new AuthService(
            context,
            Mock.Of<IPasswordHasher>(),
            tokenGenerator.Object);

        var result = await service.LogoutAsync(new LogoutRequest
        {
            RefreshToken = "valid-token",
            IpAddress = "127.0.0.1"
        });

        Assert.True(result.IsSuccess);

        var token = await context.RefreshTokens.FirstAsync();
        Assert.NotNull(token.RevokedAt);
        Assert.Equal("Logged out", token.RevokedReason);
        Assert.Equal("127.0.0.1", token.RevokedByIp);
    }
}
