namespace DevHabit.Api.DTO.HabitTag;

public sealed record  UpsertHabitTagDto
{   
    public required List<string> TagIds { get; init; }
}
