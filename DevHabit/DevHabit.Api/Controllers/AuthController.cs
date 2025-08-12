using DevHabit.Api.Database;
using DevHabit.Api.DTO.Auth;
using DevHabit.Api.DTO.Users;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevHabit.Api.Controllers;
[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController(
    UserManager<IdentityUser> userManager, 
    ApplicationDbContext dbContext, 
    ApplicationIdentityDbContext identityDbContext) : ControllerBase
{
    [HttpPost("register")]

    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());
        var identityUser = new IdentityUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Email
        };

        IdentityResult identityResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
        if (!identityResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?> { 
                { 
                    "error", 
                    identityResult.Errors.ToDictionary(e => e.Code, e=> e.Description)
                } 
            };
            return Problem(
                detail: "Unable to register uses, please try again!",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
                );
        }

       User user = registerUserDto.ToEntity();
       user.IdentityId = identityUser.Id;
       dbContext.Users.Add( user );

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return Ok(user.Id);

    }
}
