using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MasterAPI.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection service,
            //Esta interfaz me permite acceder a los valores del archivo 
            //appsettings.json donde se encuentra la cadena de conexion de
            //la base de datos
            IConfiguration conf)
        {
            service.AddDbContext<MasterAPIDbContext>(options => {
                
                //Esta configuracion muestra en consola los comandos de las
                //operaciones que se realizan en la base de datos.
                options.LogTo(Console.WriteLine, new [] {
                    DbLoggerCategory.Database.Command.Name,

                }, LogLevel.Information)
                .EnableSensitiveDataLogging();

                options.UseAsyncSeeding( static async (context, status, cancellationToken) =>
                {
                    var masterAPIDbContext = (MasterAPIDbContext)context;
                    var logger = context.GetService<ILogger<MasterAPIDbContext>>();
                    try
                    {
                        await SeedDatabase.SeedDataAsync(masterAPIDbContext, logger, cancellationToken);
                        await SeedDatabase.SeedUserAndRolesAsync(masterAPIDbContext, logger, cancellationToken);
                    }
                    catch
                    {
                        logger.LogError("Error al ejecutar el seeding asíncrono.");
                    }
                });
                options.UseSqlite(conf.GetConnectionString("SqliteDataBase"));
            });

            return service;
        }
    }
}