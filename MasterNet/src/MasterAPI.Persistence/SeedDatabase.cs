using System.Collections.Frozen;
using System.Security.Claims;
using Bogus.DataSets;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MasterAPI.Persistence
{
    public static class SeedDatabase
    {
        // Método principal para sembrar los datos en la base de datos.
        public static async Task SeedDataAsync(
            MasterAPIDbContext context,
            ILogger? logger,
            CancellationToken cancellationToken = default
        )
        {
            await SeedPreciosAsync(context, logger, cancellationToken);
            await SeedIntructoresAsync(context, logger, cancellationToken);
            await SeedCursosAsync(context, logger, cancellationToken);
            await SeedCalificacionesAsync(context, logger, cancellationToken);
        }

        //En este caso no se utiliza con contexto definido (MasterAPIDbContext), es necesario utilizar DbContext
        //para poder crear instancias de UserManager y RoleManager, a partir de DbContext. Ya que existe dentro de Identity.
        public static async Task SeedUserAndRolesAsync(
            DbContext context,
            ILogger? logger,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                //Esto me permite los servicios de UserManager para poder gestionar usuarios.
                //Y crear nuevos usuarios o gestionar los existentes a partir del modelo personalizado UserApp.
                var userManager = context.GetService<UserManager<UserApp>>();   
                //Esto me permite los servicios de RoleManager para poder gestionar roles.
                var roleManager = context.GetService<RoleManager<IdentityRole>>();

                //Si ya existen usuarios, no hago nada.
                if(userManager.Users.Any()) return;

                //Creo los Ids para los roles ADMIN y CLIENT
                var adminRoleId = Guid.NewGuid().ToString();
                var clientRoleId = Guid.NewGuid().ToString();

                //Ahora creo los objetos de roles ADMIN y CLIENT, usando los Ids generados y los nombres definidos en CustomRol.
                var roleAdmin = new IdentityRole
                {
                    Id = adminRoleId,
                    Name = CustomRol.ADMIN,
                    NormalizedName = CustomRol.ADMIN.ToUpper()
                };

                var roleClient = new IdentityRole
                {
                    Id = clientRoleId,
                    Name = CustomRol.CLIENT,
                    NormalizedName = CustomRol.CLIENT.ToUpper()
                };

                //Creo los roles en la base de datos si no existen.
                if(!await roleManager.RoleExistsAsync(CustomRol.ADMIN))
                {
                    var result = await roleManager.CreateAsync(roleAdmin);
                    if(!result.Succeeded)
                    {
                        throw new Exception("No se pudo crear el rol ADMIN o este ya existe.");
                    }
                }
                if(!await roleManager.RoleExistsAsync(CustomRol.CLIENT))
                {
                    var result = await roleManager.CreateAsync(roleClient);
                    if(!result.Succeeded)
                    {
                        throw new Exception("No se pudo crear el rol CLIENT o este ya existe.");
                    }
                }

                //Creo un usuario administrador por defecto, usando UserApp como modelo de usuario, para poder
                //tener las propiedades personalizadas como NombreCompleto y TituloProfesional.
                var userAdmin = new UserApp
                {
                    UserName = "adminmasterapi",
                    Email = "adminmasterapi@masterapi.com",
                    NombreCompleto = "Administrador MasterAPI",
                    TituloProfesional = "Ingeniero en Sistemas",
                };
                
                //Persisto el usuario administrador en la base de datos con una contraseña.
                await userManager.CreateAsync(userAdmin, "Password123@");

                //creo un usuario cliente por defecto, usando UserApp como modelo de usuario, para poder
                //tener las propiedades personalizadas como NombreCompleto y TituloProfesional.
                var userClient = new UserApp
                {
                    UserName = "clientmasterapi",
                    Email = "clientmasterapi@masterapi.com",
                    NombreCompleto = "Cliente MasterAPI",
                    TituloProfesional = "Estudiante",
                };

                //Persisto el usuario cliente en la base de datos con una contraseña.
                await userManager.CreateAsync(userClient, "Password123@");


                //Asigno el rol ADMIN al usuario administrador.
                await userManager.AddToRoleAsync(userAdmin, CustomRol.ADMIN);
                //Asigno el rol CLIENT al usuario cliente.
                await userManager.AddToRoleAsync(userClient, CustomRol.CLIENT);


                //Ahora hay que agregar los claims a los roles, para definir los permisos que cada rol tiene.
                //De esta forma, cuando un usuario tenga un rol, automáticamente tendrá los claims asociados a ese rol.
                
                //Asigno todos los claims de políticas al rol ADMIN
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.CURSO_CREATE));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.CURSO_READ));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.CURSO_UPDATE));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.CURSO_DELETE));

                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.INSTRUCTOR_CREATE));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.INSTRUCTOR_READ));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.INSTRUCTOR_UPDATE));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.INSTRUCTOR_DELETE));
                
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.COMENTARIO_CREATE));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.COMENTARIO_READ));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.COMENTARIO_UPDATE));
                await roleManager.AddClaimAsync(roleAdmin, new Claim(CustomClaims.POLICIES, PolicyMaster.COMENTARIO_DELETE));

                //Asigno solo los claims de lectura al rol CLIENT
                await roleManager.AddClaimAsync(roleClient, new Claim(CustomClaims.POLICIES, PolicyMaster.CURSO_READ));
                await roleManager.AddClaimAsync(roleClient, new Claim(CustomClaims.POLICIES, PolicyMaster.INSTRUCTOR_READ));
                await roleManager.AddClaimAsync(roleClient, new Claim(CustomClaims.POLICIES, PolicyMaster.COMENTARIO_READ));


            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error al cargar usuarios y roles.");
            }
        }
        public static async Task SeedPreciosAsync(
            MasterAPIDbContext context,
            ILogger? logger,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                //Logica de captura de datos de un JSON y deserialización
                if (context.Precios != null && context.Precios.Any())
                {
                    logger?.LogInformation("Los precios ya existen en la base de datos.");
                    return;
                }

                var jsonString = GetJsonFile("precios.json");
                var precios = JsonConvert.DeserializeObject<List<Precio>>(jsonString);

                if (precios is null || (precios?.Any() == false)) return;
                
                await context.Precios?.AddRangeAsync(precios!)!;
                await context.SaveChangesAsync(cancellationToken);
                
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error al cargar precios.");
            }
        }

        public static async Task SeedCursosAsync(
            MasterAPIDbContext context,
            ILogger? logger,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                //Logica de captura de datos de un JSON y deserialización
                if (context.Cursos != null && context.Cursos.Any())
                {
                    logger?.LogInformation("Los cursos ya existen en la base de datos.");
                    return;
                }

                var jsonString = GetJsonFile("cursos.json");

                //Se crean diccionarios para instructores y precios para luego asignarlos a los cursos.
                //Esto es necesario porque en el JSON de cursos solo tenemos los IDs de las entidades relacionadas,
                //no los objetos completos. Por lo tanto, necesitamos poder buscar rápidamente los objetos Instructor
                //y Precio por su ID, en este caso obtenemos los registros ya cargados en la base de datos y completos, 
                //no solo los IDs.
                var instructores = context.Instructores!.ToFrozenDictionary(i => i.Id, i => i);
                var precios = context.Precios!.ToFrozenDictionary(p => p.Id, p => p);
                
                //En este caso no utilizo la deserialización directa a List<Curso> porque necesito asignar las relaciones
                //y en los archivos JSON solo tengo los IDs de las entidades relacionadas, no los objetos completos. Por eso
                //utilizo JArray para parsear el JSON y luego construyo los objetos Curso manualmente.
                var arrayCursos = JArray.Parse(jsonString);
                var cursoDb = new List<Curso>();

                //Recorro cada item del array de cursos
                foreach (var item in arrayCursos)
                {
                    //Se realiza un mapeo manual de los campos del JSON a las propiedades del objeto Curso,
                    //En primer lugar, obtengo el Id que esta en el JSON y es una cadena de texto y lo convierto a Guid.
                    //Si no se puede convertir, genero un nuevo Guid.
                    var idString = item["Id"]?.ToString();
                    if(!Guid.TryParse(idString, out var id))
                        id = Guid.NewGuid();
                    //Luego obtengo el campo Titulo y Descripcion que son cadenas de texto.
                    var titutlo = item["Titulo"]?.ToString();
                    var descripcion = item["Descripcion"]?.ToString();
                    //Para la fecha de publicación, primero obtengo el string y luego intento parsearlo a DateTime.
                    DateTime? fechaPublicacion = null;
                    var fechaPublicacionString = item["FechaPublicacion"]?.ToString();
                    //Si el string no es nulo o vacío y se puede parsear, asigno el valor a la propiedad.
                    if(!string.IsNullOrWhiteSpace(fechaPublicacionString) &&
                       DateTime.TryParse(fechaPublicacionString, out var fechaParsed))
                    {
                        fechaPublicacion = fechaParsed;
                    }
                    //Creo el objeto Curso con los valores obtenidos del JSON.
                    //A las propiedades de navegación (Instructores, Precios, Calificaciones) las inicializo
                    //con listas vacías, que luego llenaré con los objetos relacionados.
                    var curso =  new Curso
                    {
                        Id = id,
                        Titulo = titutlo!,
                        Descripcion = descripcion!,
                        FechaPublicacion = fechaPublicacion,
                        Calificaciones = new List<Calificacion>(),
                        Precios = new List<Precio>(),
                        Instructores = new List<Instructor>(),
                        Imagenes =  new List<Imagen>()                      
                    };

                    //Ahora recorro los arrays de IDs de Precios e Instructores para asignar, recordemos que en el JSON
                    //solo tenemos los IDs, no los objetos completos.
                    if(item["Precios"] is JArray preciosArray)
                    {
                        //Recorro cada ID de precio en el array
                        foreach(var precioItem in preciosArray)
                        {
                            //Convierto el ID de precio a Guid
                            var precioGuid = new Guid(precioItem?.ToString()!);
                            //Busco el objeto Precio correspondiente en el diccionario de precios
                            if(precios.TryGetValue(precioGuid, out var precio))
                            {
                                //Si lo encuentro, lo agrego a la colección de Precios del curso
                                curso.Precios.Add(precio);
                            }
                        }
                    }
                    //Hago lo mismo para los Instructores, recorro el array de IDs de instructores, ya que, en el JSON
                    //solo tenemos los IDs, no los objetos completos.
                    if(item["Instructores"] is JArray instructoresArray)
                    {
                        //Recorro cada ID de instructor en el array
                        foreach(var instructorItem in instructoresArray)
                        {
                            //Convierto el ID de instructor a Guid
                            var instructorGuid = new Guid(instructorItem?.ToString()!);
                            //Busco el objeto Instructor correspondiente en el diccionario de instructores
                            if(instructores.TryGetValue(instructorGuid, out var instructor))
                            {
                                //Si lo encuentro, lo agrego a la colección de Instructores del curso
                                curso.Instructores.Add(instructor);
                            }
                        }
                    }                   
                    //Finalmente, agrego el curso construido a la lista de cursos que se van a guardar en la base de datos.
                    cursoDb.Add(curso);
                }   

                //Una vez que he construido todos los objetos Curso con sus relaciones, los agrego al contexto
                //y guardo los cambios en la base de datos.
                await context.Cursos?.AddRangeAsync(cursoDb)!;
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error al cargar productos.");
            }
        }
        public static async Task SeedIntructoresAsync(
            MasterAPIDbContext context,
            ILogger? logger,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                //Logica de captura de datos de un JSON y deserialización
                if (context.Instructores != null && context.Instructores.Any())
                {
                    logger?.LogInformation("Los instructores ya existen en la base de datos.");
                    return;
                }

                var jsonString = GetJsonFile("instructores.json");
                var instructores = JsonConvert.DeserializeObject<List<Instructor>>(jsonString);

                if (instructores is null || (instructores.Any() == false)) return;

                await context.Instructores?.AddRangeAsync(instructores!)!;
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Error al cargar instructores.");
            }
        }
        public static async Task SeedCalificacionesAsync(
            MasterAPIDbContext dbContext,
            ILogger? logger,
            CancellationToken cancellationToken
        )
        {

            try
            {
                if (dbContext.Calificaciones is null || dbContext.Calificaciones.Any()) return;
                var jsonString = GetJsonFile("calificaciones.json");

                if (jsonString is null) return;

                var calificaciones = JsonConvert.DeserializeObject<List<Calificacion>>(jsonString);

                if (calificaciones is null || calificaciones.Any()==false) return;

                foreach (var ca in calificaciones!)
                {
                    ca.Curso = null;
                }

                dbContext.Calificaciones.AddRange(calificaciones!);
                await dbContext.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Fallo cargando la data de calificaciones");
            }
        }        
        private static string GetJsonFile(string fileName)
        {
            //Se ejecuta desde la solucion
            var leerSolucion = Path.Combine(Directory.GetCurrentDirectory(), "src", "MasterAPI.Persistence","SeedData", fileName);
            //Se ejecuta desde el proyecto Persistence
            var leerProyecto = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", fileName);
            //Se ejecuta desde el contexto de la aplicación
            var leerAppContext = Path.Combine(AppContext.BaseDirectory, "SeedData", fileName);

            if(File.Exists(leerSolucion))
                return File.ReadAllText(leerSolucion);
            else if(File.Exists(leerProyecto))
                return File.ReadAllText(leerProyecto);
            else if(File.Exists(leerAppContext))
                return File.ReadAllText(leerAppContext);
            else
                throw new FileNotFoundException("No se encontró el archivo JSON de seed data.", fileName);
        }
    }
}