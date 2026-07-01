using auth.Application.Common;
using Auth.Application.Bases;
using MediatR;

namespace auth.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<TokenPairResponse>>;