using DevHabit.Api.DTO.Auth;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTO.Users;

public static class UserMappings
{

    public static User ToEntity(this RegisterUserDto registerUserDto)
    {
        return new User
        {
            Id = $"u_{Guid.CreateVersion7()}",
            Name = registerUserDto.Name,
            Email = registerUserDto.Email,
            CreatedAtUtc = DateTime.UtcNow,
        };
    }
}
