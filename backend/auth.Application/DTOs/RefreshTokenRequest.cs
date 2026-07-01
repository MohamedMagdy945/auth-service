namespace auth.Application.DTOs;

public class RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}