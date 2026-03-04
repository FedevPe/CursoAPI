using FluentValidation;
using MasterAPI.Application.Core;
using Microsoft.Extensions.DependencyInjection;

namespace MasterAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services
        )
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                conf.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblies(new[] { typeof(DependencyInjection).Assembly });

            // services.AddFluentValidationAutoValidation();
            // services.AddValidatorsFromAssemblyContaining<CursoCreateCommand>();

            services.AddAutoMapper(
                cfg => {},
                typeof(MappingProfile).Assembly
            );

            return services;
        }
    }
}