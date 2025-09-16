using System.Security.Claims;
using Fulbito.Core.Common.Entities;
using Fulbito.Core.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.AddMatch;

public class AddMatchHandler
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddMatchHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AddMatchResponse> Handle(AddMatchCommand command)
    {
        // Obtener UserId del JWT y setear en command
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return new AddMatchResponse(false, "Usuario no autenticado");
        }
        command.UserId = userId.Value;

        // Obtener la liga para validaciones adicionales
        var league = await _context.Leagues
            .FirstOrDefaultAsync(l => l.Id == command.LeagueId && l.UserId == command.UserId);

        if (league == null)
        {
            return new AddMatchResponse(false, "Liga no encontrada o no autorizada");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Crear jugadores nuevos primero
            var createdPlayers = await CreateNewPlayers(command, league);

            // 2. Crear el partido
            var match = new Match
            {
                LeagueId = command.LeagueId,
                Team1Score = command.Team1Score,
                Team2Score = command.Team2Score,
                MatchDate = command.MatchDate
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            // 3. Crear PlayerMatch para cada jugador
            await CreatePlayerMatches(command, match, createdPlayers, league);

            await transaction.CommitAsync();
            
            return new AddMatchResponse(true, "Partido agregado exitosamente", match.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new AddMatchResponse(false, $"Error al agregar partido: {ex.Message}");
        }
    }

    private async Task<Dictionary<string, Player>> CreateNewPlayers(AddMatchCommand command, League league)
    {
        var createdPlayers = new Dictionary<string, Player>();
        var newPlayersToCreate = new List<Player>();

        // Recopilar todos los jugadores nuevos (evitar duplicados)
        var allNewPlayers = command.Team1Players.Concat(command.Team2Players)
            .Where(p => p.NewPlayer != null)
            .Select(p => p.NewPlayer!)
            .GroupBy(p => $"{p.FirstName}|{p.LastName}")
            .Select(g => g.First())
            .ToList();

        foreach (var newPlayerRequest in allNewPlayers)
        {
            var player = new Player
            {
                FirstName = newPlayerRequest.FirstName,
                LastName = newPlayerRequest.LastName,
                LeagueId = league.Id
            };

            newPlayersToCreate.Add(player);
            var key = $"{newPlayerRequest.FirstName}|{newPlayerRequest.LastName}";
            createdPlayers[key] = player;
        }

        if (newPlayersToCreate.Any())
        {
            _context.Players.AddRange(newPlayersToCreate);
            await _context.SaveChangesAsync();
        }

        return createdPlayers;
    }

    private async Task CreatePlayerMatches(AddMatchCommand command, Match match, 
                                         Dictionary<string, Player> createdPlayers, League league)
    {
        var playerMatches = new List<PlayerMatch>();

        // Procesar Team 1
        ProcessTeamPlayers(command.Team1Players, 1, match, createdPlayers, league, playerMatches, command);

        // Procesar Team 2  
        ProcessTeamPlayers(command.Team2Players, 2, match, createdPlayers, league, playerMatches, command);

        _context.PlayerMatches.AddRange(playerMatches);
        await _context.SaveChangesAsync();
    }

    private static void ProcessTeamPlayers(List<PlayerInTeamRequest> teamPlayers, int teamNumber,
                                        Match match, Dictionary<string, Player> createdPlayers,
                                        League league, List<PlayerMatch> playerMatches, AddMatchCommand command)
    {
        // Determinar resultado para este equipo
        MatchResult teamResult;
        if (teamNumber == 1)
        {
        teamResult = match.Team1Score > match.Team2Score ? MatchResult.Win :
                    match.Team1Score < match.Team2Score ? MatchResult.Loss : MatchResult.Draw;
        }
        else
        {
        teamResult = match.Team2Score > match.Team1Score ? MatchResult.Win :
                    match.Team2Score < match.Team1Score ? MatchResult.Loss : MatchResult.Draw;
        }

        foreach (var playerRequest in teamPlayers)
        {
        Guid playerId;

        if (playerRequest.PlayerId.HasValue)
        {
            // Jugador existente
            playerId = playerRequest.PlayerId.Value;
        }
        else
        {
            // Jugador nuevo - obtener de los creados
            var key = $"{playerRequest.NewPlayer!.FirstName}|{playerRequest.NewPlayer.LastName}";
            playerId = createdPlayers[key].Id;
        }

        var playerMatch = new PlayerMatch
        {
            PlayerId = playerId,
            MatchId = match.Id,
            TeamNumber = teamNumber,
            Goals = league.IsGoalsEnabled ? playerRequest.Goals : 0,
            Result = teamResult,
            IsMvp = !string.IsNullOrEmpty(command.MvpPlayerId) && command.MvpPlayerId == playerId.ToString()
        };

        playerMatches.Add(playerMatch);
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }
}