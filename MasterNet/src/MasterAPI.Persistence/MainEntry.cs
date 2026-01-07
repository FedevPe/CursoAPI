//El proyecto se ejecuta a partir de este archivo para crear la base de datos

using MasterAPI.Persistence;
using MasterAPI.Persistence.Models;
using Microsoft.AspNetCore.Identity;
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

service.AddIdentityCore<UserApp>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<MasterAPIDbContext>();

// Se agrega el DbContext al contenedor de servicios, indicando cual es el contexto que se utilizará
service.AddDbContext<MasterAPIDbContext>();

// Construir el proveedor de servicios, osea que, se crea el contenedor de servicios. Internamente este se encarga
// de resolver las dependencias cuando se solicitan los servicios.
var provider = service.BuildServiceProvider();

try
{
    //Se crea un scope para obtener una instancia del DbContext
    using var scope = provider.CreateScope();
    // Se obtiene el DbContext desde el proveedor de servicios
    var context = scope.ServiceProvider.GetRequiredService<MasterAPIDbContext>();

    // Se aplica cualquier migración pendiente.
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error al crear la base de datos. {ex.Message}");
}