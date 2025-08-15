﻿using System.Security.Claims;
using DevHabit.Api.Database;
using DevHabit.Api.DTO.Users;
using DevHabit.Api.Entities;
using DevHabit.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;
[Authorize(Roles = $"{Roles.Member}")]
[Route("users")]
[ApiController]
public sealed class UsersController(ApplicationDbContext context, UserContext userContext) : ControllerBase
{

    [HttpGet("{id}")]
    [Authorize(Roles = $"{Roles.Admin}")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }
        if(id != userId)
        {
            return Forbid();
        }
        UserDto? user = await context.Users
            .Where(u => u.Id == id)
            .Select(UserQuries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {

        string? userId = await userContext.GetUserIdAsync();

        if(string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        UserDto? user = await context.Users
            .Where(u => u.Id == userId)
            .Select(UserQuries.ProjectToDto())
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
