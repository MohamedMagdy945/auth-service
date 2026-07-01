namespace auth.api.Helper;

public static class CookieHelper
{
    private const string CookieName = "refreshToken";
    private const string CookiePath = "/api/auth";

    public static void SetRefreshTokenCookie(
        HttpResponse response,
        string refreshToken,
        DateTime expires,
        bool isSecure = true)
    {
        response.Cookies.Append(CookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Strict,
            Path = CookiePath,
            Expires = expires
        });
    }

    public static void DeleteRefreshTokenCookie(HttpResponse response, bool isSecure = true)
    {
        response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Strict,
            Path = CookiePath
        });
    }
}
