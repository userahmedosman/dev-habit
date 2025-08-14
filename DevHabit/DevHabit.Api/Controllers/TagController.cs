using AutoMapper;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.Tags;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;
[Authorize]
[Route("tags")]
[ApiController]
public class TagController(ApplicationDbContext context, IMapper mapper, UserContext userContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        List<Tag> tags = await context.Tags.Where(u => u.UserId == userId).ToListAsync();


        List<TagDto> tagsDtos = mapper.Map<List<TagDto>>(tags);

        var tagCollection = new TagCollectionDto
        {
            Items = tagsDtos
        };

        return Ok(tagCollection);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var tag = await context.Tags.FirstOrDefaultAsync(u =>  u.Id ==  id && u.UserId == userId);
        if (tag == null)
        {
            return NotFound();
        }
        var tagDto = mapper.Map<TagDto>(tag);
        return Ok(tagDto);
    }
    [HttpPost]
    public async Task<ActionResult<Tag>> CreateTag(CreateTagDto createTagDto, IValidator<CreateTagDto> validator)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(createTagDto);
        
        Tag tag = mapper.Map<Tag>(createTagDto);

        tag.Id = $"t_{Guid.CreateVersion7()}";
        tag.UserId = userId;
        tag.Name = createTagDto.Name;
        tag.Description = createTagDto.Description;
        tag.CreatedAtUtc = DateTime.UtcNow;

        if (await context.Tags.AnyAsync(t => t.Name == tag.Name))
        {
            return Problem(detail:$"Tag with name '{tag.Name}' already exists.", 
                statusCode:StatusCodes.Status409Conflict);
        }

        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        TagDto tagDto = mapper.Map<TagDto>(tag);
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tagDto);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<Tag>> UpdateTag(string id, UpdateTagDto updateTagdto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var tag = await context.Tags.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
        if (tag == null)
        {
            return NotFound();
        }
        tag.Name = updateTagdto.Name;
        tag.Description = updateTagdto.Description;
        tag.UpdatedAtUtc = DateTime.UtcNow;
        
        await context.SaveChangesAsync();
        
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var tag = await context.Tags.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
        if (tag is null)
        {
            return NotFound();
        }

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
