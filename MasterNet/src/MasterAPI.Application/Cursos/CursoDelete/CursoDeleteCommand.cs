using System.Security.Cryptography.X509Certificates;
using FluentValidation;
using MasterAPI.Application.Core;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Cursos.CursoDelete
{
    public class CursoDeleteCommand
    {
        public record CursoDeleteCommandRequest(
            Guid? cursoId
        ) : IRequest<Result<Unit>>;

        internal class CursoDeleteCommandHandler(
            MasterAPIDbContext context
        ) : IRequestHandler<CursoDeleteCommandRequest, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(CursoDeleteCommandRequest request, CancellationToken cancellationToken)
            {
                var curso = await context.Cursos!
                                        .Include(x => x.Instructores)
                                        .Include(x => x.Precios)
                                        .Include(x => x.Calificaciones)
                                        .Include(x => x.Imagenes)
                                        .FirstOrDefaultAsync(x => x.Id == request.cursoId);

                if(curso is null)
                {
                    return Result<Unit>.Failure("El curso no existe");
                }

                context.Cursos!.Remove(curso);
                var result = await context.SaveChangesAsync() > 0;

                return result 
                    ? Result<Unit>.Succes(Unit.Value) 
                    : Result<Unit>.Failure("Error al eliminar el curso");
            }
        }

        public class CursoDeleteCommandRequestValidator : AbstractValidator<CursoDeleteCommandRequest>
        {
            public CursoDeleteCommandRequestValidator()
            {
                RuleFor(x => x.cursoId).NotNull().WithMessage("El existe un curso con el id ingresado.");
            }
        }
    }
}