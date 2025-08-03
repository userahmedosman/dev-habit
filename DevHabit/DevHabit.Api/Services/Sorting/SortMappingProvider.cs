namespace DevHabit.Api.Services.Sorting;

public sealed class  SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{
    public SortMapping[] GetMappings<TSources, TDestination>()
    {
        SortMappingDefinition<TSources, TDestination>? sortMappingDefinition = sortMappingDefinitions
            .OfType<SortMappingDefinition<TSources, TDestination>>()
            .FirstOrDefault();

        if(sortMappingDefinition is null)
        {
            throw new InvalidOperationException(
                $"The mapping from '{typeof(TSources).Name} into {typeof(TDestination).Name} is not defined");
        }

        return sortMappingDefinition.Mappings; 
    }

    public bool ValidateMapping<TSources, TDestination>(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        { return true; }

        var sortedFields = sort
            .Split(',')
            .Select(f => f.Trim().Split(' ')[0])
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        SortMapping[] sortMappings = GetMappings<TSources, TDestination>();
        return sortedFields.All(f => sortMappings.Any(m => m.sortField.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
}
