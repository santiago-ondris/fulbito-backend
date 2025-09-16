using Fulbito.Core.Common.Entities;
using Fulbito.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.ViewMatchups;

public class ViewMatchupsHandler
{
    private readonly ApplicationDbContext _context;

    public ViewMatchupsHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ViewMatchupsResponse> Handle(ViewMatchupsQuery query)
    {
        // 1. Verificar que la liga existe
        var league = await _context.Leagues
            .Include(l => l.Players)
            .FirstOrDefaultAsync(l => l.Slug == query.LeagueSlug);

        if (league == null)
        {
            return new ViewMatchupsResponse
            {
                Success = false,
                Message = "Liga no encontrada"
            };
        }

        // 2. Verificar que ambos jugadores existen en la liga
        var player1 = league.Players.FirstOrDefault(p => p.Id == query.Player1Id);
        var player2 = league.Players.FirstOrDefault(p => p.Id == query.Player2Id);

        if (player1 == null || player2 == null)
        {
            return new ViewMatchupsResponse
            {
                Success = false,
                Message = "Uno o ambos jugadores no encontrados en la liga"
            };
        }

        // 3. Obtener todos los partidos donde ambos jugadores participaron
        var matchIds = await _context.PlayerMatches
            .Where(pm => pm.Match.LeagueId == league.Id && 
                        (pm.PlayerId == query.Player1Id || pm.PlayerId == query.Player2Id))
            .GroupBy(pm => pm.MatchId)
            .Where(g => g.Count() == 2) // Solo partidos donde ambos jugadores participaron
            .Select(g => g.Key)
            .ToListAsync();

        // Ahora obtener los datos completos
        var matchupMatches = await _context.Matches
            .Include(m => m.PlayerMatches)
            .Where(m => matchIds.Contains(m.Id) && 
                    m.PlayerMatches.Where(pm => pm.PlayerId == query.Player1Id).First().TeamNumber !=
                    m.PlayerMatches.Where(pm => pm.PlayerId == query.Player2Id).First().TeamNumber)
            .OrderByDescending(m => m.MatchDate)
            .ToListAsync();

        // 4. Procesar estadísticas y historial
        var matchHistoryList = new List<MatchupHistory>();
        int player1Wins = 0, player2Wins = 0, draws = 0;

        foreach (var match in matchupMatches)
        {
            var player1Match = match.PlayerMatches.First(pm => pm.PlayerId == query.Player1Id);
            var player2Match = match.PlayerMatches.First(pm => pm.PlayerId == query.Player2Id);

            // Determinar resultado del enfrentamiento
            string resultDescription;
            if (player1Match.Result == player2Match.Result)
            {
                draws++;
                resultDescription = "Empate";
            }
            else if (player1Match.Result == MatchResult.Win)
            {
                player1Wins++;
                resultDescription = $"Victoria de {player1.FirstName}";
            }
            else
            {
                player2Wins++;
                resultDescription = $"Victoria de {player2.FirstName}";
            }

            // Crear entrada del historial
            var historyEntry = new MatchupHistory
            {
                MatchId = match.Id,
                MatchDate = match.MatchDate,
                Team1Score = match.Team1Score,
                Team2Score = match.Team2Score,
                Result = resultDescription,
                Player1Details = new PlayerMatchDetails
                {
                    Goals = league.IsGoalsEnabled ? player1Match.Goals : 0,
                    WasInWinningTeam = player1Match.Result == MatchResult.Win
                },
                Player2Details = new PlayerMatchDetails
                {
                    Goals = league.IsGoalsEnabled ? player2Match.Goals : 0,
                    WasInWinningTeam = player2Match.Result == MatchResult.Win
                }
            };

            matchHistoryList.Add(historyEntry);
        }

        // 5. Crear resumen de estadísticas
        var totalMatches = matchupMatches.Count;
        string statsSummary = CreateStatsSummary(player1, player2, player1Wins, player2Wins, draws, totalMatches);

        // 6. Construir respuesta
        return new ViewMatchupsResponse
        {
            Success = true,
            Message = "Enfrentamientos obtenidos exitosamente",
            Data = new MatchupData
            {
                Player1 = new PlayerSummary
                {
                    Id = player1.Id,
                    FirstName = player1.FirstName,
                    LastName = player1.LastName,
                    FullName = $"{player1.FirstName} {player1.LastName}"
                },
                Player2 = new PlayerSummary
                {
                    Id = player2.Id,
                    FirstName = player2.FirstName,
                    LastName = player2.LastName,
                    FullName = $"{player2.FirstName} {player2.LastName}"
                },
                Stats = new MatchupStats
                {
                    Player1Wins = player1Wins,
                    Player2Wins = player2Wins,
                    Draws = draws,
                    TotalMatches = totalMatches,
                    Summary = statsSummary
                },
                Matches = matchHistoryList
            }
        };
    }

    private string CreateStatsSummary(Player player1, Player player2, int player1Wins, int player2Wins, int draws, int totalMatches)
    {
        if (totalMatches == 0)
        {
            return "No hay enfrentamientos registrados";
        }

        string summary;
        if (player1Wins > player2Wins)
        {
            summary = $"{player1.FirstName} gana por {player1Wins} a {player2Wins} a {player2.FirstName}";
        }
        else if (player2Wins > player1Wins)
        {
            summary = $"{player2.FirstName} gana por {player2Wins} a {player1Wins} a {player1.FirstName}";
        }
        else
        {
            summary = $"Empate {player1Wins}-{player2Wins} entre {player1.FirstName} y {player2.FirstName}";
        }

        if (draws > 0)
        {
            summary += $" (con {draws} empate{(draws > 1 ? "s" : "")})";
        }

        return summary;
    }
}