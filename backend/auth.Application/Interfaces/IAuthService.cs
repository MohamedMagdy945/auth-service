using auth.Application.Common;
using auth.Application.DTOs;
using Auth.Application.Bases;

namespace auth.Application.Interfaces;

public interface IAuthService
{
    Task<Result<TokenPairResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<TokenPairResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken = default);
}
