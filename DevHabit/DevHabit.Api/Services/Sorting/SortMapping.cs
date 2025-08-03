namespace DevHabit.Api.Services.Sorting;

public sealed record SortMapping(string sortField, string PropertyName, bool reverse = false);
