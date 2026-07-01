using auth.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace auth.Infrastructure.Services
{
    public class ClientInfoProvider : IClientInfoProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetIpAddress()
            => _httpContextAccessor.HttpContext?.Connection
                .RemoteIpAddress?.ToString() ?? "Unknown";

        public string GetUserAgent()
            => _httpContextAccessor.HttpContext?.Request
                .Headers.UserAgent.ToString() ?? "Unknown";
    }
}
