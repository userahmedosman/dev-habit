using System.Dynamic;
using System.Linq.Expressions;
using System.Net.Mime;
using Asp.Versioning;
using AutoMapper;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.Common;
using DevHabit.Api.DTO.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DevHabit.Api.Controllers;
[Authorize]
[Route("habits")]
[ApiController]
[ApiVersion("1.0")]
[Produces(
    MediaTypeNames.Application.Json,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.JsonV2,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1,
    CustomMediaTypeNames.Application.HateoasJsonV2)]
public sealed class HabitController(
    ApplicationDbContext context, 
    IMapper mapper, 
    LinkService linkService,
    UserContext userContext) : ControllerBase
{
    private readonly IMapper mapper = mapper;

    [HttpGet]
    public async Task<IActionResult> GetHabits([FromQuery] 
    HabitQueryParameters query, 
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShaping)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        
        if (!sortMappingProvider.ValidateMapping<HabitDto, Habit>(query.Sort))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sorting parameter is not valid -> '{query.Sort}");
        }

        if (!dataShaping.Validate<HabitDto>(query.Fields))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
              detail: $"The provided data shaping field parameter is not valid -> '{query.Fields}");
        }
        query.Search ??= query.Search?.Trim().ToLower();
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<HabitDto, Habit>();

        IQueryable<HabitWithTagDto> habitQuery = context.Habits
            .Where(u => u.UserId == userId)
            .Where(h => string.IsNullOrEmpty(query.Search) || h.Name.ToLower().Contains(query.Search) || h.Description != null && h.Description.ToLower()
            .Contains(query.Search))
            .Where(t => query.Type == null || t.Type == query.Type)
            .ApplySort(query.Sort, sortMappings)
            .Select(HabitQueries.ProjectToDtoWithTags());

        List<HabitWithTagDto> habits = await habitQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var totalCount = await habitQuery.CountAsync();
        bool includeLinks = query.Accept == CustomMediaTypeNames.Application.HateoasJson;

        var paginationResult = new PaginationResult<ExpandoObject>
        {
            Items = dataShaping.ShapeCollectionData(habits, query.Fields, includeLinks ? h => CreateLinksForHabit(h.Id, query.Fields):null),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            
        };

        if (includeLinks)
        { paginationResult.Links = CreateLinksForHabits(query, paginationResult.HasNextPage, paginationResult.HasPreviousPage); }
        return Ok(paginationResult);
    }

    [HttpGet("{id}")]
    [MapToApiVersion(1.0)]
    
    public async Task<IActionResult> GetHabit(string id, string? fields, DataShapingService dataShaping, [FromHeader(Name="Accept")]string? accept)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
       
        if (!dataShaping.Validate<HabitDto>(fields))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
              detail: $"The provided data shaping field parameter is not valid -> '{fields}");
        }
        HabitWithTagDto? habit = await context.Habits
                .Where(h => h.Id == id && h.UserId == userId)
                .Select(HabitQueries.ProjectToDtoWithTags())
                .FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }
        ExpandoObject result = dataShaping.ShapeData(habit, fields);
      if(accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            List<LinkDto> links = CreateLinksForHabit(id, fields);
            result.TryAdd("links", links);
        }
        return Ok(result);
    }


    [HttpGet("{id}")]
    [ApiVersion(2.0)]
    public async Task<IActionResult> GetHabitV2(string id, string? fields, DataShapingService dataShaping, [FromHeader(Name = "Accept")] string? accept)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        if (!dataShaping.Validate<HabitWithTagDtoV2>(fields))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
              detail: $"The provided data shaping field parameter is not valid -> '{fields}");
        }
        HabitWithTagDtoV2? habit = await context.Habits
                .Where(h => h.Id == id && h.UserId ==  userId)
                .Select(HabitQueries.ProjectToDtoWithTagsV2())
                .FirstOrDefaultAsync();

        if (habit is null)
        {
            return NotFound();
        }
        ExpandoObject result = dataShaping.ShapeData(habit, fields);
        if (accept == CustomMediaTypeNames.Application.HateoasJson)
        {
            List<LinkDto> links = CreateLinksForHabit(id, fields);
            result.TryAdd("links", links);
        }
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> CreateHabit(CreateHabitDto createHabitDto, IValidator<CreateHabitDto> validator)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        await validator.ValidateAndThrowAsync(createHabitDto);

        Habit habit = mapper.Map<Habit>(createHabitDto);
        habit.Id = $"h_{Guid.CreateVersion7()}"; 
        habit.UserId = userId;
        habit.CreatedAtUtc = DateTime.UtcNow;
        habit.Status = Entities.HabitStatus.Ongoing;
        context.Habits.Add(habit);
        await context.SaveChangesAsync();
        var result = mapper.Map<HabitDto>(habit);
        result.Links = CreateLinksForHabit(habit.Id, null);
        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Habit? existingHabit = await context.Habits.FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);
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
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        Habit? existingHabit = await context.Habits.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
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
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        Habit? existingHabit = await context.Habits.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userId);
        if (existingHabit is null)
        {
            return NotFound();
        }
        context.Habits.Remove(existingHabit);
        await context.SaveChangesAsync();
        return NoContent();
    }
    private List<LinkDto> CreateLinksForHabits(HabitQueryParameters habitQueryParameters, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkDto> links = [
             linkService.Create(nameof(GetHabits), "self", HttpMethods.Get, new {
             page = habitQueryParameters.Page,
             pagesize = habitQueryParameters.PageSize,
             fields = habitQueryParameters.Fields,
             q = habitQueryParameters.Search,
              sort = habitQueryParameters.Sort,
             type = habitQueryParameters.Type,
             status = habitQueryParameters.Status,
             }),
           linkService.Create(nameof(CreateHabit), "create", HttpMethods.Post)

                    ];

        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetHabits), "next-page", HttpMethods.Get, new
            {
                page = habitQueryParameters.Page + 1,
                pagesize = habitQueryParameters.PageSize,
                fields = habitQueryParameters.Fields,
                q = habitQueryParameters.Search,
                sort = habitQueryParameters.Sort,
                type = habitQueryParameters.Type,
                status = habitQueryParameters.Status,
            }));

        }
        if (hasPreviousPage)
        {
            links.Add(linkService.Create(nameof(GetHabits), "previous-page", HttpMethods.Get, new
            {
                page = habitQueryParameters.Page - 1,
                pagesize = habitQueryParameters.PageSize,
                fields = habitQueryParameters.Fields,
                q = habitQueryParameters.Search,
                sort = habitQueryParameters.Sort,
                type = habitQueryParameters.Type,
                status = habitQueryParameters.Status,
            }));

        }

        return links;
    }
    private List<LinkDto> CreateLinksForHabit(string id, string? fields)
    {
        return [
            linkService.Create(nameof(GetHabit), "self", HttpMethods.Get, new {id, fields}),
            linkService.Create(nameof(UpdateHabit), "update", HttpMethods.Put, new {id}),
            linkService.Create(nameof(PartialUpdate), "partial-update", HttpMethods.Patch, new {id}),
            linkService.Create(nameof(DeleteHabit), "delete", HttpMethods.Delete, new {id}),
            linkService.Create(nameof(HabitTagController.UpsertHabitTags), "upsert-tags", HttpMethods.Put, new {habitId = id}, HabitTagController.Name)

                    ];
    }
}
