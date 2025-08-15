using DevHabit.Api.Database;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        var ApplicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        var ApplicationIdentityDbContext = scope.ServiceProvider.GetService<ApplicationIdentityDbContext>();

        if (ApplicationDbContext is null)
        {
            throw new InvalidOperationException("ApplicationDbContext is not registered in the service provider.");
        }

        if(ApplicationIdentityDbContext is null)
        {
            throw new InvalidOperationException("ApplicationIdentityDbContext is not registered in the service provider.");
        }

        try
        {
            await ApplicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("ApplicationDatabase Migration Applied Successfully");

            await ApplicationIdentityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("IdentityDatabase Migration Applied Successfully");
        }
        catch (Exception e)
        {
            app.Logger.LogInformation(e,"An error occured while applying database migrations");
            throw;
        }
    }

    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

        try
        {
            if(roleManager is not null)
            {
                if (!await roleManager.RoleExistsAsync(Roles.Member))
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.Member));
                }
                if (!await roleManager.RoleExistsAsync(Roles.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                }
            }

            app.Logger.LogInformation("Successfully create roles");
        }
        catch (Exception ex)
        {

            app.Logger.LogError(ex, "An error occurred while seeding initial data");
            throw;
        }
    }

   
}
