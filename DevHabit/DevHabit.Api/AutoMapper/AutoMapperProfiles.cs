using AutoMapper;
using DevHabit.Api.DTO.Habits;
using DevHabit.Api.Entities;

namespace DevHabit.Api.AutoMapper;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<HabitDto, Habit>().ReverseMap();
        CreateMap<TargetDto, Target>().ReverseMap();
        CreateMap<FrequencyDto, Frequency>().ReverseMap();
        CreateMap<MileStoneDto, MileStone>().ReverseMap();
        CreateMap<MileStoneUpdateDto, MileStone>().ReverseMap();
        CreateMap<CreateHabitDto, Habit>().ReverseMap();
        CreateMap<UpdateHabitDto, Habit>().ReverseMap();


    }
}
