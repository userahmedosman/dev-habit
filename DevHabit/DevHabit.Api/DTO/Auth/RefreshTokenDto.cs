namespace DevHabit.Api.DTO.Auth;

public sealed record RefreshTokenDto
{
    public required string RefreshToken { get; init; }
}
