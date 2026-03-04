using System.Net;
using MasterAPI.Application.Core;
using MasterAPI.Application.Cursos.CursoCreate;
using MasterAPI.Application.Cursos.CursoGet;
using MasterAPI.Application.Cursos.CursosGet;
using MasterAPI.Application.Cursos.CursoUpdate;
using MasterAPI.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MasterAPI.Application.Cursos.CursoCreate.CursoCreateCommand;
using static MasterAPI.Application.Cursos.CursoDelete.CursoDeleteCommand;
using static MasterAPI.Application.Cursos.CursoGet.GetCursoQuery;
using static MasterAPI.Application.Cursos.CursoReport.csv.CursoReportCsvQuery;
using static MasterAPI.Application.Cursos.CursosGet.GetCursosQuery;
using static MasterAPI.Application.Cursos.CursoUpdate.CursoUpdateCommand;

namespace MasterWebAPI.Controllers
{
    //En primer lugar es necesario instanciar un objeto que implementa la interfaz ISender,
    //que es proporcionada por MediatR y se utiliza para enviar comandos y consultas al   
    //administrador de comandos y consultas de MediatR.
    [ApiController]
    [Route("api/cursos")]
    public class CursosController(
        ISender sender) : ControllerBase
    {
        [AllowAnonymous] //Cualquier cliente puede acceder a este endpoint, sin necesidad de logearse
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<CursoDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PaginationCursos(
            [FromQuery] GetCursosRequest request,
            CancellationToken cancellationToken
        )
        {
            var query = new GetCursosQueryRequest { CursosRequest = request };
            var resultado =  await sender.Send(query, cancellationToken);

            return resultado.IsSucces ? Ok(resultado.Value) : NotFound();   
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CursoDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CursoGet(
            Guid id, 
            CancellationToken cancellationToken)
        {
            var query = new GetCursoQueryRequest {Id = id};
            var resultado = await sender.Send(query, cancellationToken);
            return resultado.IsSucces ? Ok(resultado.Value) : BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("reporte")]
        public async Task<IActionResult> ReporteCsv(CancellationToken token)
        {
            var query = new CursoReportQueryRequest();
            var resultado = await sender.Send(query, token);
            byte[] excelBytes = resultado.ToArray();
            return File(excelBytes, "text/csv", "cursos.csv");
        }

        [Authorize(Policy = PolicyMaster.CURSO_CREATE)]
        [HttpPost("create")]
        //Ahora este metodo retorna un resultado personalizado, ya sea para el caso de que la operacion se ejecute con exito
        //o para cuando ocurre un error.
        public async Task<ActionResult<Result<Guid>>> CursoCreate(
            //El atributo [FromForm] indica que los datos del request se esperan en el cuerpo de la solicitud HTTP
            //como datos de formulario, lo cual es útil para manejar cargas de archivos junto con otros
            [FromForm] CursoCreateRequest request, 
            //El CancellationToken permite cancelar la operación si es necesario o si el cliente aborta la solicitud.
            CancellationToken token)
        {
            var command = new CursoCreateCommandRequest(request);
            return await sender.Send(command, token);
        }

        [Authorize(Policy = PolicyMaster.CURSO_UPDATE)]
        [HttpPut]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CursoUpdate(
            [FromBody] CursoUpdateRequest request,
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new CursoUpdateCommandRequest(request, id);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSucces ? Ok(result.Value) : BadRequest();
        }

        [Authorize(Policy = PolicyMaster.CURSO_DELETE)]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Unit>> CursoDelete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var command = new CursoDeleteCommandRequest(id);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSucces ? Ok(result.Value) : BadRequest();
        }
    }
}