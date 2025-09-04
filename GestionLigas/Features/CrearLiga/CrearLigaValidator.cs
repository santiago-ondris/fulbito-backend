using FluentValidation;

namespace GestionLigas.Features.CrearLiga;

public class CrearLigaValidator : AbstractValidator<CrearLigaRequest>
{
    public CrearLigaValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la liga es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres");
            
        RuleFor(x => x.Formato)
            .NotEmpty().WithMessage("El formato es requerido");
            
        RuleFor(x => x.CantidadMinimaArqueros)
            .GreaterThanOrEqualTo(0).WithMessage("La cantidad mínima de arqueros no puede ser negativa");
            
        RuleFor(x => x.PosicionesDisponibles)
            .NotEmpty().WithMessage("Debe especificar al menos una posición");
    }
}