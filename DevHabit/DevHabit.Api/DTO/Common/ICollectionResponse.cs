namespace DevHabit.Api.DTO.Common;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}

