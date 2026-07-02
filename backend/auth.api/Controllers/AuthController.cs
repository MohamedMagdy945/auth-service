using auth.api.Helper;
using auth.Application.Common;
using auth.Application.Features.Auth.Login;
using auth.Application.Features.Auth.Logout;
using auth.Application.Features.Auth.RefreshToken;
using auth.Application.Features.Auth.Register;
using Auth.Application.Bases;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace auth.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : AppControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<AccessTokenResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await MediatorService.Send(command);

        if (!result.IsSuccess || result.Data is null)
            return ApiResponse(result);

        CookieHelper.SetRefreshTokenCookie(
            Response,
            result.Data.RefreshToken,
            result.Data.RefreshTokenExpiration,
            HttpContext.Request.IsHttps);

        var accessTokenResponse = result.Data.Adapt<AccessTokenResponse>();

        return ApiResponse(Result<AccessTokenResponse>.Success(accessTokenResponse));
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(Result<AccessTokenResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await MediatorService.Send(command);

        if (!result.IsSuccess || result.Data is null)
            return ApiResponse(result);

        CookieHelper.SetRefreshTokenCookie(
            Response,
            result.Data.RefreshToken,
            result.Data.RefreshTokenExpiration,
            HttpContext.Request.IsHttps);

        var accessTokenResponse = result.Data.Adapt<AccessTokenResponse>();

        return ApiResponse(Result<AccessTokenResponse>.Success(accessTokenResponse));
    }
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(Result<AccessTokenResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies[CookieHelper.RefreshTokenCookieName] ?? string.Empty;

        var result = await MediatorService.Send(new RefreshTokenCommand(refreshToken));

        if (!result.IsSuccess || result.Data is null)
            return ApiResponse(result);

        CookieHelper.SetRefreshTokenCookie(
            Response,
            result.Data.RefreshToken,
            result.Data.RefreshTokenExpiration,
            HttpContext.Request.IsHttps
        );

        var accessTokenResponse = result.Data.Adapt<AccessTokenResponse>();

        return ApiResponse(Result<AccessTokenResponse>.Success(accessTokenResponse));
    }

    [HttpPost("logout")]
    [EnableRateLimiting("logout")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[CookieHelper.RefreshTokenCookieName] ?? string.Empty;
        var result = await MediatorService.Send(new LogoutCommand(refreshToken));

        CookieHelper.DeleteRefreshTokenCookie(Response, HttpContext.Request.IsHttps);

        return ApiResponse(result);
    }


}
