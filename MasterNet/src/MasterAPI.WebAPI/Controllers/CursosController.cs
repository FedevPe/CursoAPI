using MasterAPI.Application.Cursos.CursoCreate;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterAPI.Application.Cursos.CursoCreate.CursoCreateCommand;

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
        [HttpPost("create")]
        public async Task<ActionResult<Guid>> CursoCreate(
            //El atributo [FromForm] indica que los datos del request se esperan en el cuerpo de la solicitud HTTP
            //como datos de formulario, lo cual es útil para manejar cargas de archivos junto con otros
            [FromForm] CursoCreateRequest request, 
            //El CancellationToken permite cancelar la operación si es necesario o si el cliente aborta la solicitud.
            CancellationToken token)
        {
            var command = new CursoCreateCommandRequest(request);
            var resultado = await sender.Send(command, token);
            return Ok(resultado);
        }
    }
}