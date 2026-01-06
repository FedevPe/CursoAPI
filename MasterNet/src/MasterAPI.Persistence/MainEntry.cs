//El proyecto se ejecuta a partir de este archivo para crear la base de datos

using MasterAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// Configurar el servicio de inyección de dependencias
var service = new ServiceCollection();

// Se configura el logging para que no registre nada durante la creación de la base de datos, esto se hace
// porque el logging es utilizado en el DbContext y hay que registrar el servicio para evitar errores.
service.AddLogging(builder =>
{
    builder.ClearProviders();
});

// Se agrega el DbContext al contenedor de servicios, indicando cual es el contexto que se utilizará
service.AddDbContext<MasterAPIDbContext>();

// Construir el proveedor de servicios
var provider = service.BuildServiceProvider();

try
{
    //Se crea un scope para obtener una instancia del DbContext
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MasterAPIDbContext>();
    await context.Database.MigrateAsync();
    Console.WriteLine("La operación se realizó con éxito.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error al crear la base de datos. {ex.Message}");
}