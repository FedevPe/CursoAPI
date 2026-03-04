using FluentValidation;
using MasterAPI.Application.Core;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Cursos.CursoUpdate
{
    public class CursoUpdateCommand
    {
        public record CursoUpdateCommandRequest(
            CursoUpdateRequest cursoRequest,
            Guid? cursoId
        ) : IRequest<Result<Guid>>;

        internal class CursoUpdateCommandHandler(
            MasterAPIDbContext context
        ): IRequestHandler<CursoUpdateCommandRequest, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(CursoUpdateCommandRequest request, CancellationToken cancellationToken)
            {
                var cursoId = request.cursoId;

                var curso = await context.Cursos!.FirstOrDefaultAsync(x => x.Id == cursoId);

                if(curso is null)
                {
                    return Result<Guid>.Failure("El curso no existe");
                }

                curso.Titulo = request.cursoRequest.Titulo;
                curso.Descripcion = request.cursoRequest.Descripcion;
                curso.FechaPublicacion = request.cursoRequest.FechaPublicacion;

                context.Entry(curso).State = EntityState.Modified;

                var result = await context.SaveChangesAsync() > 0;

                return result ? Result<Guid>.Succes(curso.Id) : Result<Guid>.Failure("Error al actualizar el curso");
            }
        }

        public class CursoUpdateCommandRequestValidator : AbstractValidator<CursoUpdateCommandRequest>
        {
            public CursoUpdateCommandRequestValidator()
            {
                RuleFor(x => x.cursoId).NotNull();
                RuleFor(x => x.cursoRequest).SetValidator(new CursoUpdateValidator());
            }
        }
    }
}