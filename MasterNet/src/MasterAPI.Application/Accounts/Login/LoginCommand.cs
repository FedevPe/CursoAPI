using FluentValidation;
using MasterAPI.Application.Core;
using MasterAPI.Application.Interfaces;
using MasterAPI.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Accounts.Login
{
    public class LoginCommand
    {
        public record LoginCommandRequest(
            LoginRequest loginRequest) 
        : IRequest<Result<Profile>>;

        internal class LoginCommandHandler(
            UserManager<UserApp> userManager,
            ITokenService tokenService
        ) : IRequestHandler<LoginCommandRequest, Result<Profile>>
        {
            public async Task<Result<Profile>> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
            {
                var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.loginRequest.Email);

                if(user is null)
                {
                    return Result<Profile>.Failure("No se encontro el usuario.");
                }

                var result = await userManager.CheckPasswordAsync(user, request.loginRequest.Password!);

                if (!result)
                {
                    return Result<Profile>.Failure("Las credenciales son incorrectas");
                }

                var profile = new Profile()
                {
                    Email = user.Email,
                    NombreCompleto = user.NombreCompleto,
                    UserName = user.UserName,
                    Token = await tokenService.CreateToken(user)
                };

                return Result<Profile>.Succes(profile);
            }
        }

        public class LoginCommandRequestValidator : AbstractValidator<LoginCommandRequest>
        {
            public LoginCommandRequestValidator()
            {
                RuleFor(x => x.loginRequest).SetValidator(new LoginValidator());
            }
        }
    }
}