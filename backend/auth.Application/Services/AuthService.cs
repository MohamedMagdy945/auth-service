using auth.Application.Common;
using auth.Application.DTOs;
using auth.Application.Interfaces;
using auth.Domain.Entities;
using Auth.Application.Bases;
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

        return Result<TokenPairResponse>.Success(tokenResponse);
    }
}
