using Microsoft.AspNetCore.Mvc;
using DevHabit.Api.Entities;
namespace DevHabit.Api.DTO.Habits;

public sealed record HabitQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Search {  get; set; }

    public Entities.HabitType? Type { get; init; }

    public Entities.HabitStatus? Status { get; init; }

    public string? Sort { get; init; }

    public string? Fields { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 2;


}
