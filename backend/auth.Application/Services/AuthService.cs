using auth.Application.Common;
using auth.Application.DTOs;
using auth.Application.Interfaces;
using auth.Domain.Constant;
using auth.Domain.Entities;
using Auth.Application.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(
        IAuthDbContext context,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<TokenPairResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var userData = await _context.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => new
            {
                User = u,
                Permissions = u.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var passwordValid = _passwordHasher.Verify(
            request.Password,
            userData?.User.PasswordHash ?? string.Empty);

        if (userData?.User is null || !userData.User.IsEnabled || !passwordValid)
            return Result<TokenPairResponse>.Unauthorized("Invalid email or password.");

        var user = userData.User;
        var tokenResponse = _tokenGenerator.GenerateTokenPair(user, userData.Permissions);

        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenGenerator.HashToken(tokenResponse.RefreshToken),
            ExpiresAt = tokenResponse.RefreshTokenExpiration,
            CreatedByIp = request.IpAddress,
            DeviceInfo = request.DeviceInfo
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<TokenPairResponse>.Success(
            tokenResponse,
            message: "Login successful",
            statusCode: StatusCodes.Status200OK);
    }

    public async Task<Result<TokenPairResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var existingUser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (existingUser is not null)
            return Result<TokenPairResponse>.Failure(
                "Email is already registered.",
                statusCode: StatusCodes.Status409Conflict);

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User(email, request.FullName, passwordHash, request.PhoneNumber)
        {
            IsEnabled = true
        };

        var defaultRole = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name == Roles.User, cancellationToken);

        if (defaultRole is null)
        {
            return Result<TokenPairResponse>.Failure("default role not found");
        }

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var tokenResponse = _tokenGenerator.GenerateTokenPair(user, permissions);

        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenGenerator.HashToken(tokenResponse.RefreshToken),
            ExpiresAt = tokenResponse.RefreshTokenExpiration,
            CreatedByIp = request.IpAddress,
            DeviceInfo = request.DeviceInfo
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<TokenPairResponse>.Success(
            tokenResponse,
            message: "Registration successful",
            statusCode: StatusCodes.Status201Created);
    }

    public async Task<Result<bool>> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenGenerator.HashToken(request.RefreshToken);

        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == refreshTokenHash &&
                rt.RevokedAt == null,
                cancellationToken);

        if (existingToken is null)
            return Result<bool>.Unauthorized("Invalid refresh token.");

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedReason = "Logged out";
        existingToken.RevokedByIp = request.IpAddress;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, message: "Logged out successfully");
    }
}
