using AutoMapper;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.Habits;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;
[Route("habits")]
[ApiController]
public sealed class HabitController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    private readonly IMapper mapper = mapper;

    [HttpGet]
    public async Task<IActionResult> GetHabits()
    {
        List<Habit> habits = await context.Habits.ToListAsync();

        var result = mapper.Map<List<HabitDto>>(habits);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHabit(string id)
    {
        Habit? habit = await context.Habits.FindAsync(id);
        if (habit is null)
        {
            return NotFound();
        }
        var result = mapper.Map<HabitDto>(habit);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateHabit([FromBody] CreateHabitDto createHabitDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Habit habit = mapper.Map<Habit>(createHabitDto);
        habit.Id = $"h_{Guid.CreateVersion7()}"; // Using CreateVersion7 for a unique ID
        habit.CreatedAtUtc = DateTime.UtcNow;
        habit.Status = Entities.HabitStatus.Ongoing;
        context.Habits.Add(habit);
        await context.SaveChangesAsync();
        var result = mapper.Map<HabitDto>(habit);
        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Habit? existingHabit = await context.Habits.FindAsync(id);
        if (existingHabit is null)
        {
            return NotFound();
        }
        mapper.Map(updateHabitDto, existingHabit);
        existingHabit.UpdatedAtUtc = DateTime.UtcNow;
        context.Habits.Update(existingHabit);
        await context.SaveChangesAsync();
        var result = mapper.Map<HabitDto>(existingHabit);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PartialUpdate(string id, [FromBody] JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? existingHabit = await context.Habits.FirstOrDefaultAsync(Id => Id.Id == id);
        if (existingHabit is null)
        {
            return NotFound();
        }
        HabitDto habitDto = mapper.Map<HabitDto>(existingHabit);
        patchDocument.ApplyTo(habitDto, ModelState);

        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }
        existingHabit.Name = habitDto.Name;
        existingHabit.Description = habitDto.Description;
        existingHabit.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]    
    public async Task<IActionResult> DeleteHabit(string id)
    {
        Habit? existingHabit = await context.Habits.FirstOrDefaultAsync(Id => Id.Id == id);
        if (existingHabit is null)
        {
            return NotFound();
        }
        context.Habits.Remove(existingHabit);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
