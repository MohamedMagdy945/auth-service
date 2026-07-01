using auth.api.Helper;
using auth.Application.Features.Auth.Login;
using Auth.Application.Bases;
using Mapster;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;

namespace auth.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(Result<AccessTokenResponse>), StatusCodes.Status200OK)]

        public async Task<IActionResult> Login(LoginCommand command)
        {
            var response = await MediatorService.Send(command);

            if (!response.IsSuccess || response.Data is null)
                return ApiResponse(response);

            CookieHelper.SetRefreshTokenCookie(
                Response,
                response.Data.RefreshToken,
                response.Data.RefreshTokenExpiration,
                HttpContext.Request.IsHttps
            );


            var accessTokenResponse = response.Data.Adapt<AccessTokenResponse>();

            var result = Result<AccessTokenResponse>.Success(accessTokenResponse);

            return ApiResponse(result);
        }
    }
}
