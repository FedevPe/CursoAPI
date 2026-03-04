using MasterAPI.Application.Core;
using MasterAPI.Application.Interfaces;
using MasterAPI.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Accounts.GetCurrentUser
{
    public class GetCurrentUserQuery
    {
        public record GetCurrentUserQueryRequest(
            GetCurrentUserRequest CurrentUserRequest
        ) : IRequest<Result<Profile>>;

        internal class GetCurrenUserQueryHandler(
            UserManager<UserApp> userManager,
            ITokenService tokenService
        ) : IRequestHandler<GetCurrentUserQueryRequest, Result<Profile>>
        {
            public async Task<Result<Profile>> Handle(GetCurrentUserQueryRequest request, CancellationToken cancellationToken)
            {
                var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.CurrentUserRequest.Email);
                
                if(user is null)
                {
                    return Result<Profile>.Failure("No se encontro el usuario.");
                }

                var profile = new Profile
                {
                    Email = user.Email,
                    NombreCompleto = user.NombreCompleto,
                    UserName = user.UserName,
                    Token = await tokenService.CreateToken(user)                    
                };

                return Result<Profile>.Succes(profile);
            }
        }
    }
}