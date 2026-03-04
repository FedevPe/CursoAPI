using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterAPI.Application.Core;
using MasterAPI.Application.Cursos.CursoGet;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Cursos.CursosGet
{
    public class GetCursosQuery
    {
        public record GetCursosQueryRequest : IRequest<Result<PagedList<CursoDTO>>>
        {
            public GetCursosRequest? CursosRequest { get; set; }
        }

        internal class GetCursosQueryHandler(
            MasterAPIDbContext context,
            IMapper mapper
        ) : IRequestHandler<GetCursosQueryRequest, Result<PagedList<CursoDTO>>>
        {
            public async Task<Result<PagedList<CursoDTO>>> Handle(GetCursosQueryRequest request, CancellationToken cancellationToken)
            {
                IQueryable<Curso> queryable = context.Cursos!
                                                    .Include(x => x.Instructores)
                                                    .Include(x => x.Calificaciones)
                                                    .Include(x => x.Precios)
                                                    .Include(x => x.Imagenes);

                var predicate = ExpressionBuilder.New<Curso>();

                if (!string.IsNullOrEmpty(request.CursosRequest!.Titulo))
                {
                    predicate = predicate.And(y => y.Titulo!.ToLower().Contains(request.CursosRequest.Titulo.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.CursosRequest!.Descripcion))
                {
                    predicate = predicate.And(y => y.Descripcion!.ToLower().Contains(request.CursosRequest.Descripcion.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.CursosRequest!.OrderBy))
                {
                    Expression<Func<Curso, object>>? orderBySelector = request.CursosRequest.OrderBy!.ToLower() switch
                    {
                        "titulo" => curso => curso.Titulo!,
                        "descripcion" => curso => curso.Descripcion!,
                        _ => curso => curso.Titulo!
                    };

                    bool orderBy = request.CursosRequest.OrderAsc.HasValue 
                            ? request.CursosRequest.OrderAsc.Value
                            : true;

                    queryable = orderBy 
                                ? queryable.OrderBy(orderBySelector)
                                : queryable.OrderByDescending(orderBySelector);
                }

                queryable = queryable.Where(predicate);

                var query = queryable.ProjectTo<CursoDTO>(mapper.ConfigurationProvider).AsQueryable();

                //Paginación

                var pagination = await PagedList<CursoDTO>.CreateAsync(query, request.CursosRequest.PageNumber, request.CursosRequest.PageSize);

                return Result<PagedList<CursoDTO>>.Succes(pagination);
            }
        }
    }
}