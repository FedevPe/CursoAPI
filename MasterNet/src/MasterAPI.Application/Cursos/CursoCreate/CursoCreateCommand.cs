using FluentValidation;
using MasterAPI.Application.Core;
using MasterAPI.Application.Interfaces;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Cursos.CursoCreate
{
    public class CursoCreateCommand
    {
        //Se crea un record para la solicitud del comando y se implementa IRequest de MediatR
        //que devuelve un Result que envuelve a un Guid (identificador único) al crear un curso.
        public record CursoCreateCommandRequest(CursoCreateRequest curso) 
        : IRequest<Result<Guid>>, ICommandBase;


        //Se crea la clase interna CursoCreateCommandHandler que implementa IRequestHandler, esto permite
        //manejar la lógica para procesar el comando de creación de curso a partir del request del cliente y 
        //devolver un Guid que seria el identificador del curso creado.
        internal class CursoCreateCommandHandler(
            MasterAPIDbContext context,
            IPhotoService photoService) : IRequestHandler<CursoCreateCommandRequest, Result<Guid>>
        {
            //Se implementa el método Handle que recibe la solicitud del comando y un token de cancelación.
            public async Task<Result<Guid>> Handle(
                CursoCreateCommandRequest request, 
                CancellationToken cancellationToken)
            {
                //Aquí se implementaría la lógica para crear el curso en la base de datos.
                //En primer lugar, es necesario tener la sesion de Entity Framework para interactuar con la base de datos.
                //Por eso se inyecta en el constructor primario de la clase CursoCreateCommandHandler.

                var cursoId = Guid.NewGuid();
                var curso = new Curso
                {
                    Id = cursoId,
                    Titulo = request.curso.Titulo,
                    Descripcion = request.curso.Descripcion,
                    FechaPublicacion = request.curso.FechaPublicacion,
                };

                if(request.curso.Foto is not null)
                {
                    var photoUploadResult = await photoService.AddPhoto(request.curso.Foto);
                    var photo = new Imagen
                    {
                        Id = Guid.NewGuid(),
                        PublicId = photoUploadResult.PublicId,
                        Url = photoUploadResult.UrlImage,
                        CursoId = cursoId
                    };

                    curso.Imagenes = new List<Imagen> {photo};
                };

                if(request.curso.InstructorId is not null)
                {
                    var instructor = await context.Instructores!.FirstOrDefaultAsync(x => x.Id == request.curso.InstructorId);

                    if(instructor is null)
                    {
                        return Result<Guid>.Failure("No se encontro el instructor");
                    }

                    curso.Instructores = new List<Instructor> {instructor};
                }

                if(request.curso.PrecioId is not null)
                {
                    var precio = await context.Precios!.FirstOrDefaultAsync(x => x.Id == request.curso.PrecioId);

                    if(precio is null)
                    {
                        return Result<Guid>.Failure("No se encontro el precio");
                    }

                    curso.Precios = new List<Precio> {precio};
                }

                context.Add(curso);
                var resultado = await context.SaveChangesAsync(cancellationToken) > 0;

                return resultado 
                            ? Result<Guid>.Succes(curso.Id)
                            : Result<Guid>.Failure("No se pudo guardar el curso.");
            }
        };

        public class CursoCreateCommandRequestValidator
        : AbstractValidator<CursoCreateCommandRequest>
        {
            public CursoCreateCommandRequestValidator()
            {
                RuleFor(x => x.curso).SetValidator(new CursoCreateValidator());
            }
        };
    }   
    
}