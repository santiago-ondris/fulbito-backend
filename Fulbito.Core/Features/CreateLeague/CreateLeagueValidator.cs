using FluentValidation;

namespace Fulbito.Core.Features.CreateLeague;

public class CreateLeagueValidator : AbstractValidator<CreateLeagueCommand>
{
    public CreateLeagueValidator()
    {
        // Datos básicos
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la liga es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.PlayersPerTeam)
            .GreaterThanOrEqualTo(5).WithMessage("Mínimo 5 jugadores por equipo")
            .LessThanOrEqualTo(11).WithMessage("Máximo 11 jugadores por equipo");

        // Jugadores iniciales
        RuleFor(x => x.Players)
            .NotEmpty().WithMessage("Debe agregar al menos un jugador inicial");

        RuleForEach(x => x.Players).ChildRules(player =>
        {
            player.RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("El nombre del jugador es requerido")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres");

            player.RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("El apellido del jugador es requerido")
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres");
        });

        // Validaciones condicionales para métricas opcionales
        When(x => x.IsGoalsEnabled, () =>
        {
            RuleFor(x => x.PointsPerGoal)
                .NotNull().WithMessage("Debe especificar puntos por gol cuando está habilitado");
        });

        When(x => x.IsWinStreakEnabled, () =>
        {
            RuleFor(x => x.PointsPerWinInStreak)
                .NotNull().WithMessage("Debe especificar puntos por racha de victoria cuando está habilitada");
        });

        When(x => x.IsLossStreakEnabled, () =>
        {
            RuleFor(x => x.PointsPerLossInStreak)
                .NotNull().WithMessage("Debe especificar puntos por racha de derrota cuando está habilitada");
        });
        
        When(x => x.IsMvpEnabled, () =>
        {
            RuleFor(x => x.PointsPerMvp)
                .NotNull().WithMessage("Debe especificar puntos por MVP cuando está habilitado");
        });
    }
}