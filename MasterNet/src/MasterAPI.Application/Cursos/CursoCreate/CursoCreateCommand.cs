using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MediatR;

namespace MasterAPI.Application.Cursos.CursoCreate
{
    public class CursoCreateCommand
    {
        //Se crea un record para la solicitud del comando y se implementa IRequest de MediatR
        //que devuelve un Guid (identificador único) al crear un curso.
        public record CursoCreateCommandRequest(CursoCreateRequest request) 
        : IRequest<Guid>;


        //Se crea la clase interna CursoCreateCommandHandler que implementa IRequestHandler, esto permite
        //manejar la lógica para procesar el comando de creación de curso a partir del request del cliente y 
        //devolver un Guid que seria el identificador del curso creado.
        internal class CursoCreateCommandHandler(
            MasterAPIDbContext context) : IRequestHandler<CursoCreateCommandRequest, Guid>
        {
            //Se implementa el método Handle que recibe la solicitud del comando y un token de cancelación.
            public async Task<Guid> Handle(
                CursoCreateCommandRequest request, 
                CancellationToken cancellationToken)
            {
                //Aquí se implementaría la lógica para crear el curso en la base de datos.
                //En primer lugar, es necesario tener la sesion de Entity Framework para interactuar con la base de datos.
                //Por eso se inyecta en el constructor primario de la clase CursoCreateCommandHandler.

                var curso = new Curso
                {
                    Id = Guid.NewGuid(),
                    Titulo = request.request.Titulo,
                    Descripcion = request.request.Descripcion,
                    FechaPublicacion = request.request.FechaPublicacion,
                };

                context.Add(curso);
                await context.SaveChangesAsync();

                return curso.Id;
            }
        };
    }   
}