using auth.Application.Common;
using Auth.Application.Bases;
using MediatR;

namespace auth.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password)
    : IRequest<Result<TokenPairResponse>>;
