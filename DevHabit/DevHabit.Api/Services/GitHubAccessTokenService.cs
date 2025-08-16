using DevHabit.Api.Database;
using DevHabit.Api.DTO.GitHub;
using DevHabit.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Services;

public sealed class GitHubAccessTokenService(ApplicationDbContext dbContext)
{
    public async Task StoreAsync(string userId,
        StoreGitHubAccessTokenDto gitHubAccessTokenDto,
        CancellationToken cancellationToken = default)
    {
        GitHubAccessToken? ExistingGitHubAccessToken = await GetAccessTokenAsync(userId, cancellationToken);

        if (ExistingGitHubAccessToken is not null)
        {
            ExistingGitHubAccessToken.Token = gitHubAccessTokenDto.AccessToken;
            ExistingGitHubAccessToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(gitHubAccessTokenDto.ExpiresInDays);
        }
        else
        {
            dbContext.GitHubAccessTokens.Add(new GitHubAccessToken
            {
                Id = $"gh_{Guid.CreateVersion7()}",
                UserId = userId,
                Token = gitHubAccessTokenDto.AccessToken,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(gitHubAccessTokenDto.ExpiresInDays)
            });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        GitHubAccessToken? ExistingGitHubAccessToken = await GetAccessTokenAsync(userId, cancellationToken);

        return ExistingGitHubAccessToken?.Token;
    }

    public async Task RevokAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        GitHubAccessToken? ExistingGitHubAccessToken = await GetAccessTokenAsync(userId, cancellationToken);

        if (ExistingGitHubAccessToken is null)
        {
            return;
        }
        dbContext.GitHubAccessTokens.Remove(ExistingGitHubAccessToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<GitHubAccessToken?> GetAccessTokenAsync(string userId, CancellationToken cancellationToken)
    {
        return await dbContext.GitHubAccessTokens.SingleOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }
}
