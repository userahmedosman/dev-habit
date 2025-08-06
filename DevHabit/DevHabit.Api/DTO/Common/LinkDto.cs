namespace DevHabit.Api.DTO.Common;

public sealed class LinkDto
{

    public required string Href { get; set; }

    public required string Rel { get; set; }

    public required string Method { get; set; }
}
