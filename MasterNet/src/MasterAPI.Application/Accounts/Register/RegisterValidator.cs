using FluentValidation;

namespace MasterAPI.Application.Accounts.Register
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("El email no puede estar vacio.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("El formato de email no es correcto.");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("El nombre de usuario no puede estar vacio.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("La contraseña es obligatoria.");
            RuleFor(x => x.NombreCompleto).NotEmpty().WithMessage("El nombre completo no puede estar vacio.");
            RuleFor(x => x.TituloProfesional).NotEmpty().WithMessage("La carrera es obligatoria.");
        }
    }
}