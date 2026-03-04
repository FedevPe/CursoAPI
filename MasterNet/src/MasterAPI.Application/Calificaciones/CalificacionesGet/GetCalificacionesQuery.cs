using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterAPI.Application.Calificaciones.GetCalificaciones;
using MasterAPI.Application.Core;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MediatR;

namespace MasterAPI.Application.Calificaciones.CalificacionesGet
{

    public class GetCalificacionesQuery
    {

        public record GetCalificacionesQueryRequest 
        :IRequest<Result<PagedList<CalificacionDTO>>>
        {
            public GetCalificacionesRequest? CalificacionesRequest {get;set;}
        }

        internal class GetCalificacionesQueryHandler(
            MasterAPIDbContext context, 
            IMapper mapper
        ) : IRequestHandler<GetCalificacionesQueryRequest, Result<PagedList<CalificacionDTO>>>
        {
            public async Task<Result<PagedList<CalificacionDTO>>> Handle(
                GetCalificacionesQueryRequest request,
                CancellationToken cancellationToken)
            {
                
                IQueryable<Calificacion> queryable = context.Calificaciones!;                

                var predicate = ExpressionBuilder.New<Calificacion>();
                
                if(!string.IsNullOrEmpty(request.CalificacionesRequest!.Alumno))
                {
                    predicate = predicate
                    .And(y => y.Alumno!.Contains(request.CalificacionesRequest.Alumno));
                }

                if(request.CalificacionesRequest.CursoId is not null)
                {
                    predicate = predicate
                    .And(y => y.CursoId== request.CalificacionesRequest.CursoId);
                }

                if(!string.IsNullOrEmpty(request.CalificacionesRequest.OrderBy))
                {
                    Expression<Func<Calificacion, object>>? orderBySelector =
                        request.CalificacionesRequest.OrderBy.ToLower() switch
                        {
                            "alumno" => x => x.Alumno!,
                            "curso" => x => x.CursoId!,
                            _ => x => x.Alumno!
                        };

                        bool orderBy = request.CalificacionesRequest.OrderAsc.HasValue
                                        ? request.CalificacionesRequest.OrderAsc.Value
                                        : true;

                        queryable = orderBy 
                                    ? queryable.OrderBy(orderBySelector)
                                    : queryable.OrderByDescending(orderBySelector);
                }

                queryable = queryable.Where(predicate);

                var calificacionQuery = queryable
                                        .ProjectTo<CalificacionDTO>(mapper.ConfigurationProvider)
                                        .AsQueryable();

                var pagination = await PagedList<CalificacionDTO>
                        .CreateAsync(
                            calificacionQuery,
                            request.CalificacionesRequest.PageNumber,
                            request.CalificacionesRequest.PageSize
                        );


                return Result<PagedList<CalificacionDTO>>.Succes(pagination);
            }
        }

    }
    public record CalificacionDTO(
        string? Alumno,
        int? Puntaje,
        string? Comentario,
        string? NombreCurso
    )
    {
        public CalificacionDTO() : this(null, null, null, null){}
    }

}