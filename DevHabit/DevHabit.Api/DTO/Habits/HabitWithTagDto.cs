using DevHabit.Api.DTO.HabitTag;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTO.Habits;

public sealed record HabitWithTagDto
{
    public required string Id { get; init; }
    public required string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public required HabitType Type { get; init; }

    public required FrequencyDto Frequency { get; init; }

    public required TargetDto Target { get; init; }

    public required HabitStatus Status { get; init; }

    public required bool IsArchived { get; init; }

    public DateOnly? EndDate { get; init; }

    public MileStoneDto? MileStone { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? LastCompletedUtc { get; init; }
    public required List<string> Tags { get; init; }

}

public sealed record HabitWithTagDtoV2
{
    public required string Id { get; init; }
    public required string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public required HabitType Type { get; init; }

    public required FrequencyDto Frequency { get; init; }

    public required TargetDto Target { get; init; }

    public required HabitStatus Status { get; init; }

    public required bool IsArchived { get; init; }

    public DateOnly? EndDate { get; init; }

    public MileStoneDto? MileStone { get; init; }

    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? LastCompletedAt { get; init; }
    public required List<string> Tags { get; init; }

}
