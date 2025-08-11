using System.Linq.Expressions;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTO.Habits;

internal static class HabitQueries
{
    public static Expression<Func<Habit, HabitDto>> ProjectToDto()
    {
        return h => new HabitDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = (HabitType)h.Type,
            Frequency = new FrequencyDto
            {
                Type = (FrequencyType)h.Frequency.Type,
                TimesPerPeriod = h.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = h.Target.Value,
                Unit = h.Target.Unit
            },
            Status = (HabitStatus)h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            MileStone = h.MileStone == null ? null : new MileStoneDto
            {
                Target = h.MileStone.Target,
                Current = h.MileStone.Current
            },
            CreatedAtUtc = h.CreatedAtUtc,
            UpdatedAtUtc = h.UpdatedAtUtc,
            LastCompletedUtc = h.LastCompletedUtc
        };
    }

    public static Expression<Func<Habit, HabitWithTagDto>> ProjectToDtoWithTags()
    {
        return h => new HabitWithTagDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = (HabitType)h.Type,
            Frequency = new FrequencyDto
            {
                Type = (FrequencyType)h.Frequency.Type,
                TimesPerPeriod = h.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = h.Target.Value,
                Unit = h.Target.Unit
            },
            Status = (HabitStatus)h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            MileStone = h.MileStone == null ? null : new MileStoneDto
            {
                Target = h.MileStone.Target,
                Current = h.MileStone.Current
            },
            CreatedAtUtc = h.CreatedAtUtc,
            UpdatedAtUtc = h.UpdatedAtUtc,
            LastCompletedUtc = h.LastCompletedUtc,
            Tags = h.Tags.Select(t => t.Name).ToList()
        };
    }

    public static Expression<Func<Habit, HabitWithTagDtoV2>> ProjectToDtoWithTagsV2()
    {
        return h => new HabitWithTagDtoV2
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Type = (HabitType)h.Type,
            Frequency = new FrequencyDto
            {
                Type = (FrequencyType)h.Frequency.Type,
                TimesPerPeriod = h.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = h.Target.Value,
                Unit = h.Target.Unit
            },
            Status = (HabitStatus)h.Status,
            IsArchived = h.IsArchived,
            EndDate = h.EndDate,
            MileStone = h.MileStone == null ? null : new MileStoneDto
            {
                Target = h.MileStone.Target,
                Current = h.MileStone.Current
            },
            CreatedAt = h.CreatedAtUtc,
            UpdatedAt = h.UpdatedAtUtc,
            LastCompletedAt = h.LastCompletedUtc,
            Tags = h.Tags.Select(t => t.Name).ToList()
        };
    }
}
