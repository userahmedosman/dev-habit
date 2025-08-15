using DevHabit.Api.Database;
using DevHabit.Api.DTO.Auth;
using DevHabit.Api.DTO.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DevHabit.Api.Controllers;
[Route("auth")]
[ApiController]
[AllowAnonymous]
public class AuthController(
    UserManager<IdentityUser> userManager, 
    ApplicationDbContext dbContext, 
    ApplicationIdentityDbContext identityDbContext, 
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options) : ControllerBase
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    [HttpPost("register")]

    public async Task<ActionResult<AccessTokenDto>> Register(RegisterUserDto registerUserDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());
        var identityUser = new IdentityUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Email
        };

        IdentityResult createUserResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
        if (!createUserResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?> { 
                { 
                    "error", 
                    createUserResult.Errors.ToDictionary(e => e.Code, e=> e.Description)
                } 
            };
            return Problem(
                detail: "Unable to register uses, please try again!",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
                );
        }
       IdentityResult addToRoleResult = await userManager.AddToRoleAsync(identityUser, Roles.Member);

        if (!addToRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?> {
                {
                    "error",
                    addToRoleResult.Errors.ToDictionary(e => e.Code, e=> e.Description)
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
        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email, [Roles.Member]);

        AccessTokenDto accessToken = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add( refreshToken );
        await identityDbContext.SaveChangesAsync();

        await transaction.CommitAsync();

       
        return Ok(accessToken);

    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> LogIn(LogInUserDto logInUserDto)
    {
        IdentityUser? identityUser = await userManager.FindByEmailAsync(logInUserDto.Email);
        if(identityUser is null || !await userManager.CheckPasswordAsync(identityUser, logInUserDto.Password))
        {
            return Unauthorized();
        }

       IList<string> roles = await userManager.GetRolesAsync(identityUser);
        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, roles);
        AccessTokenDto accessToken = tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        identityDbContext.RefreshTokens.Add(refreshToken);
        await identityDbContext.SaveChangesAsync();

        return Ok(accessToken);
    }

    [HttpPost("refresh")]

    public async Task<ActionResult<AccessTokenDto>> Refresh(RefreshTokenDto refreshTokenDto)
    {
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token ==  refreshTokenDto.RefreshToken);

        if(refreshToken is null)
        {
            return Unauthorized();
        }

        if(refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Unauthorized();
        }
        IList<string> roles = await userManager.GetRolesAsync(refreshToken.User);
        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!, roles);

        AccessTokenDto accessToken = tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessToken.refreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);

        await identityDbContext.SaveChangesAsync();

        return Ok(accessToken);

    }
}
