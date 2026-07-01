using auth.Application.Common;
using auth.Application.Interfaces;
using auth.Domain.Entities;
using Auth.Application.Bases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace auth.Application.Features.Auth.Login;

public class LoginCommandHandler :
    IRequestHandler<LoginCommand, Result<TokenPairResponse>>
{
    private readonly IAuthDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAuthDbContext context,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IClientInfoProvider clientInfoProvider,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _clientInfoProvider = clientInfoProvider;
        _logger = logger;
    }

    public async Task<Result<TokenPairResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
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
        {
            _logger.LogWarning("Login failed. Reason: Invalid credentials.");
            return Result<TokenPairResponse>.Unauthorized("Invalid email or password.");
        }

        var user = userData.User;
        var tokenResponse = _tokenGenerator.GenerateTokenPair(user, userData.Permissions);

        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == user.Id
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = "Replaced by new login";
        }

        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenGenerator.HashToken(tokenResponse.RefreshToken),
            ExpiresAt = tokenResponse.RefreshTokenExpiration,
            CreatedByIp = _clientInfoProvider.GetIpAddress(),
            DeviceInfo = _clientInfoProvider.GetUserAgent()
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} logged in successfully.", user.Id);

        return Result<TokenPairResponse>.Success(tokenResponse);
    }
}
