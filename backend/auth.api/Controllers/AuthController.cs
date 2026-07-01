using auth.api.Helper;
using auth.Application.Common;
using auth.Application.Features.Auth.Login;
using Auth.Application.Bases;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace auth.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        [HttpPost("login")]
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

            return ApiResponse(Result<AccessTokenResponse>.Success(
                result.Data.Adapt<AccessTokenResponse>()));
        }
    }
}
