using MasterAPI.Application;
using MasterAPI.Application.Interfaces;
using MasterAPI.Insfraestructure.Photos;
using MasterAPI.Insfraestructure.Reports;
using MasterAPI.Persistence;
using MasterAPI.WebAPI.Extensions;
using MasterAPI.WebAPI.Middleware;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication();
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddIdentityServices(builder.Configuration);
        builder.Services.AddPoliciesServices();

        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));

        builder.Services.AddScoped<IPhotoService, PhotoServices>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped(typeof(IReportService<>), typeof(ReportService<>));
        
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        // builder.Services.AddOpenApi();

        builder.Services.AddControllers();
        builder.Services.AddSwaggerDocumentation();

        // builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddSwaggerGen();          

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        // {
        //     app.MapOpenApi();
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }

        app.UseSwaggerDocumentation();

        //ASP.NET incluye los servicios de autenticación y autorización
        app.UseAuthentication();
        app.UseAuthorization();
        
        await app.SeedDataAuthentication();

        //Permite identificar los controladores en la aplicación y relacionarlos con las rutas HTTP correspondientes.
        app.MapControllers();
        app.Run();
    }
}