using DevHabit.Api.Database;
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

}
