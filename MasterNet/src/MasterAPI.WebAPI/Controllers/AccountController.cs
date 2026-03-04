using System.Net;
using MasterAPI.Application.Accounts;
using MasterAPI.Application.Accounts.GetCurrentUser;
using MasterAPI.Application.Accounts.Login;
using MasterAPI.Application.Accounts.Register;
using MasterAPI.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MasterAPI.Application.Accounts.GetCurrentUser.GetCurrentUserQuery;
using static MasterAPI.Application.Accounts.Login.LoginCommand;
using static MasterAPI.Application.Accounts.Register.RegisterCommand;

namespace MasterAPI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController(
        ISender sender,
        IUserAccessor userAccessor
    ) : ControllerBase
    {
        [AllowAnonymous] //Cualquier cliente puede acceder a este endpoint, sin necesidad de logearse
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Profile>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new LoginCommandRequest (request);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSucces 
                   ? Ok(result.Value)
                   : Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Profile>> Register(
            [FromForm] RegisterRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new RegisterCommandRequest(request);
            var result = await sender.Send(command, cancellationToken);
            return result.IsSucces
                   ? Ok(result.Value)
                   : Unauthorized();
        }

        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<Profile>> GetCurrentUser(
            CancellationToken cancellationToken
        )
        {
            var email = userAccessor.GetEmail();
            var request = new GetCurrentUserRequest {Email = email};
            var query = new GetCurrentUserQueryRequest(request);
            var result = await sender.Send(query, cancellationToken);

            return result.IsSucces
                   ? Ok(result.Value)
                   : Unauthorized();
        }
    }
}