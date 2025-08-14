using System.Security.Claims;

namespace DevHabit.Api.Extensions;

public static class ClamisPrincipalExtensions
{
    public static string? GetIdentityId(this ClaimsPrincipal? claimsPrincipal)
    {
        string? identityId = claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return identityId;
    }
}
