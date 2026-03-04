using MasterAPI.Domain.Models;

namespace MasterAPI.WebAPI.Extensions
{
    public static class PoliciesConfiguration
    {
        public static IServiceCollection AddPoliciesServices(
            this IServiceCollection services
        )
        {
            services.AddAuthorization(opt =>
            {
                //CURSO
                opt.AddPolicy(
                    PolicyMaster.CURSO_CREATE, //Nombre de la policy
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.CURSO_CREATE))
                    //Creacion del objeto, que busca dentro del token (seccion de policies dentro del los claims) si el valor coincide con CURSO_CREATE, en este caso.
                );
                opt.AddPolicy(
                    PolicyMaster.CURSO_READ, //Nombre de la policy
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.CURSO_READ))
                    //Creacion del objeto, que busca dentro del token (seccion de policies dentro del los claims) si el valor coincide con CURSO_READ, en este caso.
                );
                //Para las demas policies se utiliza la misma estructura, solo se cambia el nombre de la policy que se va a registrar.
                opt.AddPolicy(
                    PolicyMaster.CURSO_UPDATE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.CURSO_UPDATE))
                );
                opt.AddPolicy(
                    PolicyMaster.CURSO_DELETE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.CURSO_DELETE))
                );
                //INSTRUCTOR
                opt.AddPolicy(
                    PolicyMaster.INSTRUCTOR_CREATE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.INSTRUCTOR_CREATE))
                );
                opt.AddPolicy(
                    PolicyMaster.INSTRUCTOR_READ, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.INSTRUCTOR_READ))
                );
                opt.AddPolicy(
                    PolicyMaster.INSTRUCTOR_UPDATE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.INSTRUCTOR_UPDATE))
                );
                opt.AddPolicy(
                    PolicyMaster.INSTRUCTOR_DELETE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.INSTRUCTOR_DELETE))
                );
                //CALIFICACIONES O COMENTARIOS
                opt.AddPolicy(
                    PolicyMaster.COMENTARIO_CREATE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.COMENTARIO_CREATE))
                );
                opt.AddPolicy(
                    PolicyMaster.COMENTARIO_READ, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.COMENTARIO_READ))
                );
                opt.AddPolicy(
                    PolicyMaster.COMENTARIO_UPDATE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.COMENTARIO_UPDATE))
                );
                opt.AddPolicy(
                    PolicyMaster.COMENTARIO_DELETE, 
                    p => p.RequireAssertion(context => context.User.HasClaim(claim => claim.Type == CustomClaims.POLICIES && claim.Value == PolicyMaster.COMENTARIO_DELETE))
                );
                //
            });

            return services;
        }
    }
}