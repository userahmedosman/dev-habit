using AutoMapper;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.Tags;
using DevHabit.Api.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;
[Route("tags")]
[ApiController]
public class TagController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetTags()
    {
        var tags = await context.Tags.ToListAsync();


        var tagsDtos = mapper.Map<List<TagDto>>(tags);

        var tagCollection = new TagCollectionDto
        {
            Items = tagsDtos
        };

        return Ok(tagCollection);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        var tag = await context.Tags.FirstOrDefaultAsync(Id =>  Id.Id ==  id);
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
        await validator.ValidateAndThrowAsync(createTagDto);
        
        var tag = mapper.Map<Tag>(createTagDto);

        tag.Id = $"t_{Guid.CreateVersion7()}";
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
        
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<Tag>> UpdateTag(string id, UpdateTagDto updateTagdto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var tag = await context.Tags.FirstOrDefaultAsync(Id => Id.Id == id);
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
        var tag = await context.Tags.FirstOrDefaultAsync(Id => Id.Id == id);
        if (tag is null)
        {
            return NotFound();
        }

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
