using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterAPI.Application.Calificaciones.CalificacionesGet;
using MasterAPI.Application.Core;
using MasterAPI.Application.Imagenes.ImagenGet;
using MasterAPI.Application.Instructores.InstructoresGet;
using MasterAPI.Application.Precios.PreciosGet;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MasterAPI.Application.Cursos.CursoGet.GetCursoQuery;

namespace MasterAPI.Application.Cursos.CursoGet
{
    public class GetCursoQuery
    {
        public record GetCursoQueryRequest
        : IRequest<Result<CursoDTO>>
        {
            public Guid Id { get; set; }
        }    
    }

    internal class GetCursoQueryHandler(
        MasterAPIDbContext context, 
        IMapper mapper)
    : IRequestHandler<GetCursoQueryRequest, Result<CursoDTO>>
    {
        public async Task<Result<CursoDTO>> Handle(GetCursoQueryRequest request, CancellationToken cancellationToken)
        {
            var curso = await context.Cursos!.Where(x => x.Id == request.Id)
                .Include(x => x.Instructores)
                .Include(x => x.Precios)
                .Include(x => x.Calificaciones)
                .Include(x => x.Imagenes)
                .ProjectTo<CursoDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return Result<CursoDTO>.Succes(curso!);
        }
    }
    public record CursoDTO (
        Guid? Id,
        string? Titulo,
        string? Descripcion,
        DateTime? FechaPublicacion,
        List<ImagenDTO>? Imagenes,
        List<InstructorDTO>? Instructores,
        List<CalificacionDTO>? Calificaciones,
        List<PrecioDTO>? Precios
    )
    {
        public CursoDTO() : this(null, null, null, null, null, null, null, null){}
    }
}
    