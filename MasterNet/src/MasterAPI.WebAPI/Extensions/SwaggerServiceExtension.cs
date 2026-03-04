using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.OpenApi.Models;

namespace MasterAPI.WebAPI.Extensions
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(
            this IServiceCollection services
        )
        {
            services.AddEndpointsApiExplorer();
            _ = services.AddSwaggerGen(c =>
            {
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authoritation",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                //El securityRequirement lo que hace es agregar en la UI de Swagger una ventana
                //donde insertar el token para poder acceder a los endpoints.
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        securitySchema, new [] {"Bearer"}
                    }
                };

                c.AddSecurityRequirement(securityRequirement);

            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(
            this IApplicationBuilder app
        )
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}