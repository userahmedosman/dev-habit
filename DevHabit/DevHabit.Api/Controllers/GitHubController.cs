using DevHabit.Api.DTO.Common;
using DevHabit.Api.DTO.GitHub;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DevHabit.Api.Controllers;
[Authorize(Roles = Roles.Member)]
[Route("github")]
[ApiController]
public sealed class GitHubController(GitHubAccessTokenService gitHubAccessTokenService,
    GitHubService gitHubService, UserContext userContext, LinkService linkService ) : ControllerBase
{
    [HttpPut("personal-access-token")]
    public async Task<ActionResult> StoreAccessToken(StoreGitHubAccessTokenDto storeGitHubAccessTokenDto)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.StoreAsync(userId, storeGitHubAccessTokenDto);

        return NoContent();
    }

    [HttpDelete("personal-access-token")]
    public async Task<ActionResult> RevokeAccessToken()
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await gitHubAccessTokenService.RevokAsync(userId);

        return NoContent();
    }

    [HttpGet("profile")]
    public async Task<ActionResult<GitHubUserProfileDto>> GetUserProfile([FromHeader] AcceptHeaderDto acceptHeaderDto)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        string? accessToken = await  gitHubAccessTokenService.GetAsync(userId);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return NotFound();
        }

        GitHubUserProfileDto? userProfileDto = await gitHubService.GetUserProfileAsync(accessToken);
        if(userProfileDto is null)
        {
            return NotFound();
        }
        if (acceptHeaderDto.IncludeLinks)
        {
            userProfileDto.Links = [
           linkService.Create(nameof(GetUserProfile), "self", HttpMethods.Get),
            linkService.Create(nameof(StoreAccessToken), "put", HttpMethods.Put),
            linkService.Create(nameof(RevokeAccessToken), "delete", HttpMethods.Delete),
            ];
        }

        return Ok(userProfileDto);
    } 
}
