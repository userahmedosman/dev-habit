namespace DevHabit.Api.DTO.Habits;

public sealed record UpdateHabitDto
{
    public required string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public required HabitType Type { get; init; }

    public required FrequencyDto Frequency { get; init; }

    public required TargetDto Target { get; init; }

    public DateOnly? EndDate { get; init; }

    public MileStoneUpdateDto? MileStone { get; init; }
}
public sealed class MileStoneUpdateDto
{
    public required int Target { get; init; }

}
