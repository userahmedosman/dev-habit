﻿namespace DevHabit.Api.DTO.Auth;

public sealed record LogInUserDto
{
    public required string Email { get; init; }

    public required string Password { get; init; }

}
