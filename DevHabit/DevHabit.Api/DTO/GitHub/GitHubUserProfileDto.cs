﻿using DevHabit.Api.DTO.Common;
using Newtonsoft.Json;

namespace DevHabit.Api.DTO.GitHub;

public sealed record GitHubUserProfileDto(
    [property: JsonProperty("login")] string Login,
    [property: JsonProperty("name")] string? Name,
    [property: JsonProperty("avatar_url")] string AvatarUrl,
    [property: JsonProperty("bio")] string? Bio,
    [property: JsonProperty("public_repos")] int PublicRepos,
    [property: JsonProperty("followers")] int Followers,
    [property: JsonProperty("following")] int Following
) : ILinksResponce
{
    public List<LinkDto> Links { get; set; }
}
