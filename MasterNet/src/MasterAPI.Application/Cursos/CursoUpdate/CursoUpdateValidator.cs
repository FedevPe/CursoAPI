using FluentValidation;

namespace MasterAPI.Application.Cursos.CursoUpdate
{
    public class CursoUpdateValidator : AbstractValidator<CursoUpdateRequest>
    {
        public CursoUpdateValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage("El titulo no debe estar vacio");
            RuleFor(x => x.Descripcion).NotEmpty().WithMessage("La descripción no debe estar vacia");
            RuleFor(x => x.FechaPublicacion).NotEmpty().WithMessage("La fecha de publicación no debe estar vacia");
        }
    }
}