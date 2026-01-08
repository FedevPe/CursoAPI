using MasterAPI.Application;
using MasterAPI.Persistence;
using MasterAPI.WebAPI.Extensions;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication();
        builder.Services.AddPersistence(builder.Configuration);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();          

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        await app.SeedDataAuthentication();

        //Permite identificar los controladores en la aplicación y relacionarlos con las rutas HTTP correspondientes.
        app.MapControllers();
        app.Run();
    }
}