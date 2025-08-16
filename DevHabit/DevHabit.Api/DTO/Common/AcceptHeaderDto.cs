using System.Net.Http.Headers;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.DTO.Common;

public record AcceptHeaderDto
{
    [FromHeader(Name = "Accept")]
    public string? Accept { get; init; }

    public bool IncludeLinks =>
      MediaTypeHeaderValue.TryParse(Accept, out MediaTypeHeaderValue? mediaType) &&
      !string.IsNullOrEmpty(mediaType.MediaType) &&
      mediaType.MediaType
        .Contains(CustomMediaTypeNames.Application.HateoasSubType);
}
