using MasterAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.WebAPI.Extensions
{
    public static class DataSeed
    {
        public static async Task SeedDataAuthentication(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateAsyncScope();
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();

            try
            {
                var context = service.GetRequiredService<MasterAPIDbContext>();
                await  context.Database.MigrateAsync();                
            }
            catch (Exception e)
            {
                var logger = loggerFactory.CreateLogger<MasterAPIDbContext>();
                logger.LogError(e.Message);
            }
        }
    }
}