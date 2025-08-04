using DevHabit.Api.DTO.Common;

namespace DevHabit.Api.DTO.Tags;

public sealed record TagCollectionDto : ICollectionResponse<TagDto>
{
    public List<TagDto> Items { get; init; }
}
