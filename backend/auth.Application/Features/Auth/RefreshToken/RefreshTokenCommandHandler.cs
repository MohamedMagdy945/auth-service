using auth.Application.Common;
using auth.Application.Interfaces;
using Auth.Application.Bases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace auth.Application.Features.Auth.RefreshToken;

public class RefreshTokenCommandHandler :
    IRequestHandler<RefreshTokenCommand, Result<TokenPairResponse>>
{
    private readonly IAuthService _authService;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IAuthService authService,
        IClientInfoProvider clientInfoProvider,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _authService = authService;
        _clientInfoProvider = clientInfoProvider;
        _logger = logger;
    }

    public async Task<Result<TokenPairResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(
            request.RefreshToken, 
            cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Token refresh failed. Reason: {Reason}",
                result.Message);
            return result;
        }

        _logger.LogInformation(
            "Token refreshed successfully for user {UserId}.", 
            result.Data!.UserId);

        return result;
    }
}