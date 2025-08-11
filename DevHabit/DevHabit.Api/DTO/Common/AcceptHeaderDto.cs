using System.Net.Http.Headers;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.DTO.Common;

public record AcceptHeaderDto
{
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }

    //public bool IncludeLinks =>
    //    MediaTypeHeaderValue.TryParse(Accept, out MediaTypeHeaderValue? mediatype) &&
    //    mediatype.SubTypeWithOutSuffix.HasValue && 
    //    mediatype.SubTypeWithOutSuffix.Value.Contains(CustomMediaTypeNames.Application.Ha)

}
