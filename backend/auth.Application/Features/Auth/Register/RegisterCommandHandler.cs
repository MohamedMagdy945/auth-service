using auth.Application.Common;
using auth.Application.DTOs;
using auth.Application.Interfaces;
using Auth.Application.Bases;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace auth.Application.Features.Auth.Register;

public class RegisterCommandHandler :
    IRequestHandler<RegisterCommand, Result<TokenPairResponse>>
{
    private readonly IAuthService _authService;
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IAuthService authService,
        IClientInfoProvider clientInfoProvider,
        ILogger<RegisterCommandHandler> logger)
    {
        _authService = authService;
        _clientInfoProvider = clientInfoProvider;
        _logger = logger;
    }

    public async Task<Result<TokenPairResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var registerRequest = request.Adapt<RegisterRequest>();
        registerRequest.IpAddress = _clientInfoProvider.GetIpAddress();
        registerRequest.DeviceInfo = _clientInfoProvider.GetUserAgent();

        var result = await _authService.RegisterAsync(registerRequest, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Registration failed for email {Email}. Reason: {Reason}",
                request.Email.Trim().ToLowerInvariant(),
                result.Message);
            return result;
        }

        _logger.LogInformation("User {UserId} registered successfully.", result.Data!.UserId);

        return result;
    }
}