using FluentValidation;
using MasterAPI.Application.Core;
using MasterAPI.Application.Interfaces;
using MasterAPI.Persistence;
using MasterAPI.Persistence.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Accounts.Register
{
    public class RegisterCommand
    {
        public record RegisterCommandRequest(
            RegisterRequest RegisterRequest
        ): IRequest<Result<Profile>>;

        internal class RegisterCommandHandler(
            UserManager<UserApp> userManager,
            ITokenService tokenService
        ) : IRequestHandler<RegisterCommandRequest, Result<Profile>>
        {
            public async Task<Result<Profile>> Handle(
                RegisterCommandRequest request, 
                CancellationToken cancellationToken)
            {
                if(await userManager.Users.AnyAsync(x => x.Email == request.RegisterRequest.Email))
                {
                    Result<Profile>.Failure("El email ya fue registrado por otro usuario.");
                }
                if(await userManager.Users.AnyAsync(x => x.UserName == request.RegisterRequest.UserName))
                {
                    Result<Profile>.Failure("El nombre de usuario ya fue registrado.");
                }

                var user = new UserApp
                {
                    Id = Guid.NewGuid().ToString(),
                    NombreCompleto = request.RegisterRequest.NombreCompleto,
                    Email = request.RegisterRequest.Email,
                    UserName = request.RegisterRequest.UserName,
                    TituloProfesional = request.RegisterRequest.TituloProfesional
                };

                var resultado = await userManager.CreateAsync(user, request.RegisterRequest.Password!);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Client");
                
                    var profile = new Profile
                    {
                        Email = user.Email,
                        NombreCompleto = user.NombreCompleto,
                        UserName = user.UserName,  
                        Token = await tokenService.CreateToken(user)
                    };

                    return Result<Profile>.Succes(profile);
                }

                return Result<Profile>.Failure("Ha ocurrido un error al registrar el usuario.");
            }

            public class RegisterCommandRequestValidator : AbstractValidator<RegisterCommandRequest>
            {
                public RegisterCommandRequestValidator()
                {
                    RuleFor(x => x.RegisterRequest).SetValidator(new RegisterValidator());
                }
            }
        }
    }
}