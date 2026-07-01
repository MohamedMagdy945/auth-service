using auth.Application.Common;
using auth.Application.Interfaces;
using auth.Domain.Entities;
using auth.Infrastructure.settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace auth.Infrastructure.Services;

public class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenGenerator(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public TokenPairResponse GenerateTokenPair(User user, IEnumerable<string>? permissions)
    {
        return new TokenPairResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = GenerateAccessToken(user, permissions),
            RefreshToken = GenerateRefreshToken(),
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
        };
    }

    public string GenerateAccessToken(User user, IEnumerable<string>? permissions)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("perm_version", user.PermissionsVersion.ToString())
        };

        if (permissions is not null && permissions.Any())
        {
            claims.AddRange(permissions.Select(p => new Claim("permission", p)));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: creds);

        return _tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public string HashToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenSecret);

        using var hmac = new HMACSHA256(key);
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes).ToLower();
    }
}
