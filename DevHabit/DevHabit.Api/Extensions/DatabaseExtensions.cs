using DevHabit.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

        if (dbContext == null)
        {
            throw new InvalidOperationException("ApplicationDbContext is not registered in the service provider.");
        }

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database Migration Applied Successfully");
        }
        catch (Exception e)
        {
            app.Logger.LogInformation(e,"An error occured while applying database migrations");
            throw;
        }
    }

}
