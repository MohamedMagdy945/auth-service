using auth.Application.Common;
using auth.Application.DTOs;
using Auth.Application.Bases;

namespace auth.Application.Interfaces
{
    public interface IAuthService
    {
        //Task<Result<TokenResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<Result<TokenResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
        //Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
        //Task<Result<bool>> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
    }
}
