using auth.Application.Common;
using auth.Application.DTOs;
using auth.Application.Interfaces;
using Auth.Application.Bases;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace auth.Application.Features.Auth.Login;

public class LoginCommandHandler :
    IRequestHandler<LoginCommand, Result<TokenPairResponse>>
{
    private readonly IAuthService _authService;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IAuthService authService,
        IClientInfoProvider clientInfoProvider,
        ILogger<LoginCommandHandler> logger)
    {
        _authService = authService;
        _clientInfoProvider = clientInfoProvider;
        _logger = logger;
    }

    public async Task<Result<TokenPairResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var loginRequest = request.Adapt<LoginRequest>();
        loginRequest.IpAddress = _clientInfoProvider.GetIpAddress();
        loginRequest.DeviceInfo = _clientInfoProvider.GetUserAgent();

        var result = await _authService.LoginAsync(loginRequest, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Login failed for email {Email}. Reason: {Reason}",
                request.Email.Trim().ToLowerInvariant(),
                result.Message);
            return result;
        }

        _logger.LogInformation("User {UserId} logged in successfully.", result.Data!.UserId);

        return result;
    }
}
