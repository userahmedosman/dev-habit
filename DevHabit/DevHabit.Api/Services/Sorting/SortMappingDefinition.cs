namespace DevHabit.Api.Services.Sorting;

public sealed class SortMappingDefinition<TSources, TDestination>: ISortMappingDefinition
{
    public required SortMapping[] Mappings { get; init; }    
}
