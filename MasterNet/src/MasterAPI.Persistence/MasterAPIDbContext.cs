using MasterAPI.Domain.Models;
using MasterAPI.Persistence.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Persistence
{
    public class MasterAPIDbContext : IdentityDbContext<UserApp> 
    {
        public DbSet<Curso>? Cursos {get;set;}
        public DbSet<Instructor>? Instructores {get;set;}
        public DbSet<Precio>? Precios {get;set;}
        public DbSet<Calificacion>? Calificaciones {get;set;}

        // Constructor sin parámetros, se utiliza para herramientas de migración de EF Core. Es decir,
        // es utilizado por el proyecto de Persistence cuando se ejecutan comandos de migración desde la consola de
        // administración de paquetes o la CLI de .NET.
        public MasterAPIDbContext() {}

        // Constructor para inyección de dependencias, se utiliza para crear el contexto con opciones específicas
        // y es llamado por el contenedor de inyección de dependencias. La configuracion de acceso a la base de datos
        // va depender de otros proyectos (por ejemplo, API o pruebas unitarias).
        public MasterAPIDbContext(DbContextOptions<MasterAPIDbContext> options) : base(options){}

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite("Data Source=masterapi.db")
        //     .LogTo(Console.WriteLine, LogLevel.Information)
        //     .EnableSensitiveDataLogging()
        //     .UseAsyncSeeding(static async (context, status, cancelationToken) =>
        //     {
        //         var masterAPIDbContext = (MasterAPIDbContext)context;
        //         var logger = context.GetService<ILogger<MasterAPIDbContext>>();
        //         try
        //         {
        //            await SeedDatabase.SeedDataAsync(masterAPIDbContext, logger, cancelationToken);
        //            await SeedDatabase.SeedUserAndRolesAsync(masterAPIDbContext, logger, cancelationToken);
        //         }
        //         catch
        //         {
        //             logger.LogError("Error al ejecutar el seeding asíncrono.");
        //         }
        //     });
        // }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //Configuración de Nombres de tablas
            modelBuilder.Entity<Curso>().ToTable("cursos");
            modelBuilder.Entity<Instructor>().ToTable("instructores");
            modelBuilder.Entity<CursoInstructor>().ToTable("cursoinstructores");
            modelBuilder.Entity<Precio>().ToTable("precios");
            modelBuilder.Entity<CursoPrecio>().ToTable("cursoprecios");

            modelBuilder.Entity<Imagen>().ToTable("imagenes");
            modelBuilder.Entity<Calificacion>().ToTable("calificaciones");

            //Configuración de columnas 
            modelBuilder.Entity<Precio>()
                .Property(p => p.Nombre)
                .HasColumnType("VARCHAR")
                .HasMaxLength(250);
            modelBuilder.Entity<Precio>()
                .Property(p => p.PrecioActual)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Precio>()
                .Property(p => p.PrecioPromocion)
                .HasPrecision(10, 2);

            //Configuración de relaciones
            modelBuilder.Entity<Curso>()
                .HasMany( c => c.Imagenes)
                .WithOne(i => i.Curso)
                .HasForeignKey(i => i.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Calificaciones)
                .WithOne(cal => cal.Curso)
                .HasForeignKey(cal => cal.CursoId)
                .OnDelete(DeleteBehavior.Restrict);    

            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Precios)
                .WithMany(c => c.Cursos)
                .UsingEntity<CursoPrecio>(
                    cp => cp
                        .HasOne(p => p.Precio)
                        .WithMany(p => p.CursoPrecios)
                        .HasForeignKey(p => p.PrecioId),
                    cp => cp
                        .HasOne(c => c.Curso)
                        .WithMany(c => c.CursoPrecios)
                        .HasForeignKey(c => c.CursoId),
                    cp => cp
                        .HasKey(x => new { x.CursoId, x.PrecioId })
                );
            modelBuilder.Entity<Curso>()
                .HasMany(c => c.Instructores)
                .WithMany(ci => ci.Cursos)
                .UsingEntity<CursoInstructor>(
                    ci => ci
                        .HasOne(ci => ci.Instructor)
                        .WithMany(i => i.CursoInstructores)
                        .HasForeignKey(ci => ci.InstructorId),
                    ci => ci
                        .HasOne(ci => ci.Curso)
                        .WithMany(c => c.CursoInstructores)
                        .HasForeignKey(ci => ci.CursoId),
                    ci => ci
                        .HasKey(x => new { x.CursoId, x.InstructorId })
                );
        }
    }
}