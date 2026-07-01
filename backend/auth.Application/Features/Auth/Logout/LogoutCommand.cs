using Auth.Application.Bases;
using MediatR;

namespace auth.Application.Features.Auth.Logout;

public record LogoutCommand(string RefreshToken)
    : IRequest<Result<bool>>;
