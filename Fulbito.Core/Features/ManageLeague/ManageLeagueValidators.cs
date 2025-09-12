using FluentValidation;
using Fulbito.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.ManageLeague;

// ========== ADD PLAYER VALIDATOR ==========
public class AddPlayerValidator : AbstractValidator<AddPlayerCommand>
{
    private readonly ApplicationDbContext _context;

    public AddPlayerValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("El ID de la liga es requerido");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres");

        RuleFor(x => x)
            .MustAsync(LeagueExistsAndBelongsToUser).WithMessage("Liga no encontrada o no autorizada")
            .MustAsync(PlayerNameNotDuplicatedInLeague).WithMessage("Ya existe un jugador con ese nombre en la liga");
    }

    private async Task<bool> LeagueExistsAndBelongsToUser(AddPlayerCommand command, CancellationToken cancellationToken)
    {
        return await _context.Leagues.AnyAsync(l => l.Id == command.LeagueId && l.UserId == command.UserId, cancellationToken);
    }

    private async Task<bool> PlayerNameNotDuplicatedInLeague(AddPlayerCommand command, CancellationToken cancellationToken)
    {
        return !await _context.Players.AnyAsync(p => 
            p.LeagueId == command.LeagueId && 
            p.FirstName.ToLower() == command.FirstName.ToLower() && 
            p.LastName.ToLower() == command.LastName.ToLower(), 
            cancellationToken);
    }
}

// ========== EDIT PLAYER VALIDATOR ==========
public class EditPlayerValidator : AbstractValidator<EditPlayerCommand>
{
    private readonly ApplicationDbContext _context;

    public EditPlayerValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("El ID de la liga es requerido");

        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("El ID del jugador es requerido");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido")
            .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres");

        RuleFor(x => x)
            .MustAsync(LeagueExistsAndBelongsToUser).WithMessage("Liga no encontrada o no autorizada")
            .MustAsync(PlayerExistsInLeague).WithMessage("Jugador no encontrado en la liga")
            .MustAsync(PlayerNameNotDuplicatedInLeague).WithMessage("Ya existe otro jugador con ese nombre en la liga");
    }

    private async Task<bool> LeagueExistsAndBelongsToUser(EditPlayerCommand command, CancellationToken cancellationToken)
    {
        return await _context.Leagues.AnyAsync(l => l.Id == command.LeagueId && l.UserId == command.UserId, cancellationToken);
    }

    private async Task<bool> PlayerExistsInLeague(EditPlayerCommand command, CancellationToken cancellationToken)
    {
        return await _context.Players.AnyAsync(p => p.Id == command.PlayerId && p.LeagueId == command.LeagueId, cancellationToken);
    }

    private async Task<bool> PlayerNameNotDuplicatedInLeague(EditPlayerCommand command, CancellationToken cancellationToken)
    {
        return !await _context.Players.AnyAsync(p => 
            p.LeagueId == command.LeagueId && 
            p.Id != command.PlayerId && // Excluir el jugador actual
            p.FirstName.ToLower() == command.FirstName.ToLower() && 
            p.LastName.ToLower() == command.LastName.ToLower(), 
            cancellationToken);
    }
}

// ========== DELETE PLAYER VALIDATOR ==========
public class DeletePlayerValidator : AbstractValidator<DeletePlayerCommand>
{
    private readonly ApplicationDbContext _context;

    public DeletePlayerValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("El ID de la liga es requerido");

        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("El ID del jugador es requerido");

        RuleFor(x => x)
            .MustAsync(LeagueExistsAndBelongsToUser).WithMessage("Liga no encontrada o no autorizada")
            .MustAsync(PlayerExistsInLeague).WithMessage("Jugador no encontrado en la liga")
            .MustAsync(PlayerHasNoMatches).WithMessage("No se puede eliminar un jugador que ya participó en partidos. Para mantener la integridad de las estadísticas históricas, los jugadores con partidos jugados no pueden ser eliminados.");
    }

    private async Task<bool> LeagueExistsAndBelongsToUser(DeletePlayerCommand command, CancellationToken cancellationToken)
    {
        return await _context.Leagues.AnyAsync(l => l.Id == command.LeagueId && l.UserId == command.UserId, cancellationToken);
    }

    private async Task<bool> PlayerExistsInLeague(DeletePlayerCommand command, CancellationToken cancellationToken)
    {
        return await _context.Players.AnyAsync(p => p.Id == command.PlayerId && p.LeagueId == command.LeagueId, cancellationToken);
    }

    private async Task<bool> PlayerHasNoMatches(DeletePlayerCommand command, CancellationToken cancellationToken)
    {
        return !await _context.PlayerMatches.AnyAsync(pm => pm.PlayerId == command.PlayerId, cancellationToken);
    }
}