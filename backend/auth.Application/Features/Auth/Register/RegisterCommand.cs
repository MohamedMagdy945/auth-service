using auth.Application.Common;
using Auth.Application.Bases;
using MediatR;

namespace auth.Application.Features.Auth.Register;

public record RegisterCommand(string FullName, string Email, string PhoneNumber, string Password, string ConfirmPassword)
    : IRequest<Result<TokenPairResponse>>;
