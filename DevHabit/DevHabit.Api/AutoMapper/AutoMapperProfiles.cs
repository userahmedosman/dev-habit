using AutoMapper;
using DevHabit.Api.DTO.Habits;
using DevHabit.Api.DTO.HabitTag;
using DevHabit.Api.DTO.Tags;
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
        CreateMap<HabitWithTagDto, Habit>().ReverseMap()
            .ForMember(dest => dest.Tags, opt =>
            opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()));

        // Tags

        CreateMap<TagDto, Tag>().ReverseMap();
        CreateMap<CreateTagDto, Tag>().ReverseMap();
        CreateMap<UpdateTagDto, Tag>().ReverseMap();

        // Habit Tags
     

    }
}

