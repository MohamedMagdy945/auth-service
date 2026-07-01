using auth.Application.Common;
using auth.Application.DTOs;
using Auth.Application.Bases;

namespace auth.Application.Interfaces;

public interface IAuthService
{
    Task<Result<TokenPairResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
