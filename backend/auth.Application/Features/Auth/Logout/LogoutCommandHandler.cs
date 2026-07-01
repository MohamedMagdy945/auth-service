using auth.Application.DTOs;
using auth.Application.Interfaces;
using Auth.Application.Bases;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace auth.Application.Features.Auth.Logout;

public class LogoutCommandHandler :
    IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IAuthService _authService;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IAuthService authService,
        IClientInfoProvider clientInfoProvider,
        ILogger<LogoutCommandHandler> logger)
    {
        _authService = authService;
        _clientInfoProvider = clientInfoProvider;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var logoutRequest = request.Adapt<LogoutRequest>();
        logoutRequest.IpAddress = _clientInfoProvider.GetIpAddress();
        logoutRequest.DeviceInfo = _clientInfoProvider.GetUserAgent();

        var result = await _authService.LogoutAsync(logoutRequest, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Logout failed. Reason: {Reason}", result.Message);
            return result;
        }

        _logger.LogInformation("User logged out successfully.");

        return result;
    }
}
