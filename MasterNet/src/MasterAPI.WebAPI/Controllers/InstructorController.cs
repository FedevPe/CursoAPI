using MasterAPI.Application.Instructores.InstructoresGet;
using MasterAPI.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MasterAPI.Application.Instructores.InstructoresGet.GetInstructoresQuery;

namespace MasterAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/instructores")]
    public class InstructorController(
        ISender sender
    ) : ControllerBase
    {
        [Authorize(Policy = PolicyMaster.INSTRUCTOR_READ)]
        [HttpGet]
        public async Task<IActionResult> PaginationInstructores(
            [FromQuery] GetInstructoresRequest request,
            CancellationToken cancellationToken
        )
        {
            var query = new GetInstructoresQueryRequest { InstructoresRequest = request };
            var resultado = await sender.Send(query, cancellationToken);

            return resultado.IsSucces ? Ok(resultado.Value) : NotFound();
        }
    }
}