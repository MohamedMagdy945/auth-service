using auth.Application.Common;
using Auth.Application.Bases;
using MediatR;

namespace auth.Application.Features.Auth.Login
{
    public record LoginCommand : IRequest<Result<TokenResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
