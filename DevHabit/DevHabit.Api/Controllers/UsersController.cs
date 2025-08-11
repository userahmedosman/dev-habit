using DevHabit.Api.Database;
using DevHabit.Api.DTO.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;
[Route("users")]
[ApiController]
internal sealed class UsersController(ApplicationDbContext context) : ControllerBase
{

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        UserDto? user = await context.Users
            .Where(u => u.Id == id)
            .Select(UserQuries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
