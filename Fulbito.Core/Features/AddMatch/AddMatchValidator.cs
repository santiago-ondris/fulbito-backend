using FluentValidation;
using Fulbito.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.AddMatch;

public class AddMatchValidator : AbstractValidator<AddMatchCommand>
{
    private readonly ApplicationDbContext _context;

    public AddMatchValidator(ApplicationDbContext context)
    {
        _context = context;
        
        // Validaciones básicas
        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("El ID de la liga es requerido");

        RuleFor(x => x.Team1Score)
            .GreaterThanOrEqualTo(0).WithMessage("El puntaje del equipo 1 no puede ser negativo");

        RuleFor(x => x.Team2Score)
            .GreaterThanOrEqualTo(0).WithMessage("El puntaje del equipo 2 no puede ser negativo");

        RuleFor(x => x.MatchDate)
            .NotEmpty().WithMessage("La fecha del partido es requerida");

        // Validaciones de equipos
        RuleFor(x => x.Team1Players)
            .NotEmpty().WithMessage("El equipo 1 debe tener al menos un jugador");

        RuleFor(x => x.Team2Players)
            .NotEmpty().WithMessage("El equipo 2 debe tener al menos un jugador");

        // Validaciones individuales de jugadores
        RuleForEach(x => x.Team1Players).ChildRules(ValidatePlayer);
        RuleForEach(x => x.Team2Players).ChildRules(ValidatePlayer);

        // Validaciones que requieren acceso a la base de datos
        RuleFor(x => x)
            .MustAsync(LeagueExistsAndBelongsToUser).WithMessage("La liga no existe o no pertenece al usuario")
            .MustAsync(PlayersPerTeamMatchConfiguration).WithMessage("La cantidad de jugadores por equipo no coincide con la configuración de la liga")
            .MustAsync(AllPlayerIdsExistInLeague).WithMessage("Uno o más jugadores no pertenecen a la liga")
            .MustAsync(NoRepeatedPlayers).WithMessage("Un jugador no puede estar en ambos equipos")
            .MustAsync(MvpPlayerIsInMatch).WithMessage("El jugador MVP debe estar participando en el partido");
    }

    private void ValidatePlayer(InlineValidator<PlayerInTeamRequest> playerValidator)
    {
        // Debe tener playerId O newPlayer, pero no ambos ni ninguno
        playerValidator.RuleFor(p => p)
            .Must(p => (p.PlayerId.HasValue && p.NewPlayer == null) || 
                      (!p.PlayerId.HasValue && p.NewPlayer != null))
            .WithMessage("Cada jugador debe ser existente (PlayerId) o nuevo (NewPlayer), pero no ambos");

        // Validar datos del jugador nuevo
        playerValidator.When(p => p.NewPlayer != null, () =>
        {
            playerValidator.RuleFor(p => p.NewPlayer!.FirstName)
                .NotEmpty().WithMessage("El nombre del jugador nuevo es requerido")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres");

            playerValidator.RuleFor(p => p.NewPlayer!.LastName)
                .NotEmpty().WithMessage("El apellido del jugador nuevo es requerido")
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres");
        });

        // Goles no pueden ser negativos
        playerValidator.RuleFor(p => p.Goals)
            .GreaterThanOrEqualTo(0).WithMessage("Los goles no pueden ser negativos");
    }

    private async Task<bool> LeagueExistsAndBelongsToUser(AddMatchCommand command, CancellationToken cancellationToken)
    {
        return await _context.Leagues.AnyAsync(l => l.Id == command.LeagueId && l.UserId == command.UserId, cancellationToken);
    }

    private async Task<bool> PlayersPerTeamMatchConfiguration(AddMatchCommand command, CancellationToken cancellationToken)
    {
        var league = await _context.Leagues.FirstOrDefaultAsync(l => l.Id == command.LeagueId, cancellationToken);
        
        if (league == null) return false;

        return command.Team1Players.Count == league.PlayersPerTeam && 
               command.Team2Players.Count == league.PlayersPerTeam;
    }

    private async Task<bool> AllPlayerIdsExistInLeague(AddMatchCommand command, CancellationToken cancellationToken)
    {
        var existingPlayerIds = command.Team1Players.Concat(command.Team2Players)
            .Where(p => p.PlayerId.HasValue)
            .Select(p => p.PlayerId!.Value)
            .Distinct()
            .ToList();

        if (!existingPlayerIds.Any()) return true; // No hay jugadores existentes para validar

        var existingPlayersCount = await _context.Players
            .CountAsync(p => p.LeagueId == command.LeagueId && existingPlayerIds.Contains(p.Id), cancellationToken);

        return existingPlayersCount == existingPlayerIds.Count;
    }

    private async Task<bool> NoRepeatedPlayers(AddMatchCommand command, CancellationToken cancellationToken)
    {
        var allPlayerIds = command.Team1Players.Concat(command.Team2Players)
            .Where(p => p.PlayerId.HasValue)
            .Select(p => p.PlayerId!.Value)
            .ToList();

        // Verificar que no hay IDs duplicados
        if (allPlayerIds.Count != allPlayerIds.Distinct().Count())
            return false;

        var newPlayers = command.Team1Players.Concat(command.Team2Players)
            .Where(p => p.NewPlayer != null)
            .Select(p => $"{p.NewPlayer!.FirstName}|{p.NewPlayer.LastName}")
            .ToList();

        return await Task.Run(() => newPlayers.Count == newPlayers.Distinct().Count());
    }

    private async Task<bool> MvpPlayerIsInMatch(AddMatchCommand command, CancellationToken cancellationToken)
    {
        if (!command.MvpPlayerId.HasValue)
            return true;

        var allPlayerIds = await Task.Run(() => command.Team1Players.Concat(command.Team2Players)
            .Where(p => p.PlayerId.HasValue)
            .Select(p => p.PlayerId!.Value)
            .ToList());

        return allPlayerIds.Contains(command.MvpPlayerId.Value);
    }
}