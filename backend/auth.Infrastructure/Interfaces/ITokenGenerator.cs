using auth.Application.Common;
using auth.Domain.Entities;

namespace auth.Infrastructure.Interfaces
{
    public interface ITokenGenerator
    {
        TokenPairResponse GenerateTokenPair(User user, IEnumerable<string>? permissions);
        string GenerateAccessToken(User user, IEnumerable<string>? permissions);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}