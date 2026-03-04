using FluentValidation;

namespace MasterAPI.Application.Accounts.Login
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotNull().WithMessage("El email no debe estar vacio.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("El email ingresado no corresponde a un formato correcto.");
            RuleFor(x => x.Password).NotNull().WithMessage("La contraseña no debe estar vacia.");
        }
    }
}