using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterAPI.Application.Core;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Instructores.InstructoresGet
{
    public class GetInstructoresQuery
    {
        public record GetInstructoresQueryRequest : IRequest<Result<PagedList<InstructorDTO>>>
        {
            public GetInstructoresRequest? InstructoresRequest { get; set; }
        }

        internal class GetInstructoresQueryHandler (
            MasterAPIDbContext context,
            IMapper mapper
        )
        : IRequestHandler<GetInstructoresQueryRequest, Result<PagedList<InstructorDTO>>>
        {
            public async Task<Result<PagedList<InstructorDTO>>> Handle(GetInstructoresQueryRequest request, CancellationToken cancellationToken)
            {
                IQueryable<Instructor> queryable = context.Instructores!
                                                        .Include(x => x.CursoInstructores!)
                                                        .ThenInclude(ci => ci.Curso);
                
                var predicate = ExpressionBuilder.New<Instructor>();

                if (!string.IsNullOrEmpty(request.InstructoresRequest!.Nombre))
                {
                    predicate = predicate.And(y => y.Nombre!.ToLower().Contains(request.InstructoresRequest.Nombre.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.InstructoresRequest!.Apellido))
                {
                    predicate = predicate.And(y => y.Apellido!.ToLower().Contains(request.InstructoresRequest.Apellido.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.InstructoresRequest!.OrderBy))
                {
                    Expression<Func<Instructor, object>>? orderBySelector = request.InstructoresRequest.OrderBy!.ToLower() switch
                    {
                        "nombre" => instructores => instructores.Nombre!,
                        "apellido" => instructores => instructores.Apellido!,
                        _ => instructores => instructores.Nombre!,
                    };

                    bool orderBy = request.InstructoresRequest.OrderAsc.HasValue
                            ? request.InstructoresRequest.OrderAsc.Value
                            : true;

                    queryable = orderBy
                                ? queryable.OrderBy(orderBySelector)
                                : queryable.OrderByDescending(orderBySelector);
                }

                queryable = queryable.Where(predicate);

                var query = queryable.ProjectTo<InstructorDTO>(mapper.ConfigurationProvider).AsQueryable();

                var pagination = await PagedList<InstructorDTO>.CreateAsync(query, request.InstructoresRequest.PageNumber,
                                    request.InstructoresRequest.PageSize);

                return Result<PagedList<InstructorDTO>>.Succes(pagination);                                                        
            }
        }
    }
    public record InstructorDTO(
        Guid? Id,
        string? Nombre, 
        string? Apellido,
        string? Grado
    )
    {
        public InstructorDTO() : this(null, null, null, null){}
    }
}