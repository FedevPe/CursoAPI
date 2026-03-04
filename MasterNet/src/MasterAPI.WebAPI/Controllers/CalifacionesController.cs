using System.Net;
using MasterAPI.Application.Calificaciones.CalificacionesGet;
using MasterAPI.Application.Calificaciones.GetCalificaciones;
using MasterAPI.Application.Core;
using MasterAPI.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MasterAPI.Application.Calificaciones.CalificacionesGet.GetCalificacionesQuery;

namespace MasterAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/calificacion")]
    public class CalificaionController(
        ISender sender
    ) : ControllerBase
    {
        [Authorize(Policy = PolicyMaster.COMENTARIO_READ)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedList<CalificacionDTO>>> PaginationCalifacion(
            [FromQuery] GetCalificacionesRequest request,
            CancellationToken cancellationToken
        )
        {
            var query = new GetCalificacionesQueryRequest {CalificacionesRequest = request};
            var result = await sender.Send(query, cancellationToken);
            return result.IsSucces
                   ? Ok(result.Value)
                   : NotFound();

        }
    }
}