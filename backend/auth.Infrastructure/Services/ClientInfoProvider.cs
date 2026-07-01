using auth.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace auth.Infrastructure.Services;

public class ClientInfoProvider : IClientInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
            return "Unknown";

        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
            return forwarded.Split(',')[0].Trim();

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    public string GetUserAgent()
        => _httpContextAccessor.HttpContext?.Request
            .Headers.UserAgent.ToString() ?? "Unknown";
}
