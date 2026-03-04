using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MasterAPI.Application.Core;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MasterNet.Application.Precios.GetPrecios;
using MediatR;

namespace MasterAPI.Application.Precios.PreciosGet
{


    public class GetPreciosQuery
    {

        public record GetPreciosQueryRequest 
        : IRequest<Result<PagedList<PrecioDTO>>>
        {
            public GetPreciosRequest? PreciosRequest {get;set;} 
        }

        internal class GetPreciosQueryHandler(
            MasterAPIDbContext context,
            IMapper mapper
        ): IRequestHandler<GetPreciosQueryRequest, Result<PagedList<PrecioDTO>>>
        {

            public async Task<Result<PagedList<PrecioDTO>>> Handle(
                GetPreciosQueryRequest request, 
                CancellationToken cancellationToken
            )
            {
            
                IQueryable<Precio> queryable = context.Precios!;

                var predicate = ExpressionBuilder.New<Precio>();

                if(!string.IsNullOrEmpty(request.PreciosRequest!.Nombre))
                {   
                    predicate  = predicate
                    .And(y => y.Nombre!.Contains(request.PreciosRequest!.Nombre));
                }

                if(!string.IsNullOrEmpty(request.PreciosRequest!.OrderBy))
                {
                    Expression<Func<Precio, object>>? orderSelector = 
                        request.PreciosRequest.OrderBy.ToLower() switch
                        {
                            "nombre" => x => x.Nombre!,
                            "precio" => x => x.PrecioActual,
                            _ =>x => x.Nombre!
                        };

                        bool orderBy = request.PreciosRequest.OrderAsc.HasValue
                            ? request.PreciosRequest.OrderAsc.Value
                            : true;
                        
                        queryable = orderBy
                                    ? queryable.OrderBy(orderSelector)
                                    : queryable.OrderByDescending(orderSelector);
                }

                queryable = queryable.Where(predicate);

                var preciosQuery = queryable
                        .ProjectTo<PrecioDTO>(mapper.ConfigurationProvider)
                        .AsQueryable();
            

            var pagination = await PagedList<PrecioDTO>
                .CreateAsync(preciosQuery, 
                    request.PreciosRequest.PageNumber, 
                    request.PreciosRequest.PageSize
            );

            return Result<PagedList<PrecioDTO>>.Succes(pagination);
            }
        }
    }

    public record PrecioDTO(
        Guid? Id,
        string? Nombre,
        decimal? PrecioActual,
        decimal? PrecioPromocion
    )
    {
        public PrecioDTO() : this(null, null, null, null){}
    }
}