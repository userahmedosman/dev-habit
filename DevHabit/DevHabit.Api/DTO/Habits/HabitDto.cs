using DevHabit.Api.DTO.Common;

namespace DevHabit.Api.DTO.Habits;
public sealed record HabitDto: ILinksResponce
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

    public List<LinkDto> Links { get; set; }
}

public enum HabitType
{
    None = 0,
    Binary = 1,
    Measureable = 2
}

public sealed class FrequencyDto
{
    public required FrequencyType Type { get; init; }
    public required int TimesPerPeriod { get; init; }
}

public enum FrequencyType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,

}

public sealed class TargetDto
{
    public required int Value { get; init; }

    public required string Unit { get; init; }
}


public enum HabitStatus
{
    None = 0,
    Ongoing = 1,
    Completed = 2
}

public sealed class MileStoneDto
{
    public required int Target { get; init; }

    public required int Current { get; init; }
}

