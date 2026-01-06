using System.Collections.Frozen;
using MasterAPI.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MasterAPI.Persistence
{
    public static class SeedDatabase
    {
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