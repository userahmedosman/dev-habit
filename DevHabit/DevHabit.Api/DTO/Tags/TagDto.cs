namespace DevHabit.Api.DTO.Tags;
public sealed record TagDto
{
    public required string Id { get; init; }
    public required string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
