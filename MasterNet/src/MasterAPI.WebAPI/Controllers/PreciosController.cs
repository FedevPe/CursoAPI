using System.Net;
using MasterAPI.Application.Core;
using MasterAPI.Application.Precios.PreciosGet;
using MasterNet.Application.Precios.GetPrecios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MasterAPI.Application.Precios.PreciosGet.GetPreciosQuery;

namespace MasterAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/precios")]
    public class PreciosController(
        ISender sender
    ) : ControllerBase
    {   
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<PrecioDTO>>> PaginationPrice(
            [FromQuery] GetPreciosRequest request,
            CancellationToken cancellationToken
        )
        {
            var query = new GetPreciosQueryRequest { PreciosRequest = request};
            var result = await sender.Send(query, cancellationToken);
            return result.IsSucces
                   ? Ok(result.Value)
                   : NotFound();
        }
        
    }
}