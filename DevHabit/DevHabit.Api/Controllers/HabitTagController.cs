using AutoMapper;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.HabitTag;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;
[Authorize]
[Route("habits/{habitId}/tags")]
[ApiController]
public class HabitTagController(ApplicationDbContext context, UserContext userContext) : ControllerBase
{
    public static readonly string Name = nameof(HabitTagController).Replace("Controller", string.Empty);
    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagDto upsertHabitTagDto)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var habit = await context.Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
        if (habit is null)
        {
            return NotFound();
        }

        var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();

        if(currentTagIds.SetEquals(upsertHabitTagDto.TagIds))
        {
            return NoContent(); // No changes needed
        }

        List<string> existingTagIds = await context.Tags
            .Where(t => upsertHabitTagDto.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        if(existingTagIds.Count != upsertHabitTagDto.TagIds.Count)
        {
            return BadRequest("Some tags do not exist.");
        }

        habit.HabitTags.RemoveAll(ht => !upsertHabitTagDto.TagIds.Contains(ht.TagId));

        string[] tagIdsToAdd = upsertHabitTagDto.TagIds.Except(currentTagIds).ToArray();

        habit.HabitTags.AddRange(
            tagIdsToAdd.Select(tagId => new HabitTag
            {
                
                HabitId = habitId,
                TagId = tagId,
                CreatedAtUtc = DateTime.UtcNow
            })
        );

        await context.SaveChangesAsync();

        return Ok();

    }

    [HttpDelete("{tagId}")]

    public async Task<ActionResult> DeleteHabitTag(string habitId, string tagId)
    {
        var habitTag = await context.HabitTags
            .SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);
        if (habitTag is null)
        {
            return NotFound();
        }

        context.HabitTags.Remove(habitTag);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
