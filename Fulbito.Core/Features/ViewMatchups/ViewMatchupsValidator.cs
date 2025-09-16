using FluentValidation;

namespace Fulbito.Core.Features.ViewMatchups;

public class ViewMatchupsValidator : AbstractValidator<ViewMatchupsQuery>
{
    public ViewMatchupsValidator()
    {
        RuleFor(x => x.LeagueSlug)
            .NotEmpty().WithMessage("El slug de la liga es requerido")
            .MaximumLength(100).WithMessage("El slug no puede exceder 100 caracteres");

        RuleFor(x => x.Player1Id)
            .NotEmpty().WithMessage("El ID del primer jugador es requerido");

        RuleFor(x => x.Player2Id)
            .NotEmpty().WithMessage("El ID del segundo jugador es requerido");

        RuleFor(x => x)
            .Must(PlayersAreDifferent).WithMessage("No se puede comparar un jugador consigo mismo");
    }

    private bool PlayersAreDifferent(ViewMatchupsQuery query)
    {
        return query.Player1Id != query.Player2Id;
    }
}