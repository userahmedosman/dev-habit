namespace DevHabit.Api.DTO.Auth;

public sealed record TokenRequest(string userId, string email, IEnumerable<string> Roles);
