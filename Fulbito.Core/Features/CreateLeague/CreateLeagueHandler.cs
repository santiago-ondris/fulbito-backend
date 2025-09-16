using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Fulbito.Core.Common.Entities;
using Fulbito.Core.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.CreateLeague;

public class CreateLeagueHandler
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CreateLeagueHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CreateLeagueResponse> Handle(CreateLeagueCommand command)
    {
        var userId = GetCurrentUserId();
        if(userId == null)
        {
            return new CreateLeagueResponse(false, "Usuario no autenticado");
        }

        var baseSlug = GenerateSlug(command.Name);
        var slugExists = await _context.Leagues.AnyAsync(l => l.Slug == baseSlug);

        if(slugExists)
        {
            return new CreateLeagueResponse(false,
                $"Ya existe una liga con el nombre '{command.Name}'. Por favor, elegi un nombre diferente."
            );
        }

        var league = new League
        {
            Name = command.Name,
            Slug = baseSlug,
            PlayersPerTeam = command.PlayersPerTeam,
            UserId = userId.Value,
            
            // Puntajes obligatorios
            PointsPerWin = command.PointsPerWin,
            PointsPerDraw = command.PointsPerDraw,
            PointsPerLoss = command.PointsPerLoss,
            PointsPerMatchPlayed = command.PointsPerMatchPlayed,
            
            // Métricas opcionales
            IsGoalsEnabled = command.IsGoalsEnabled,
            PointsPerGoal = command.PointsPerGoal,
            IsWinStreakEnabled = command.IsWinStreakEnabled,
            PointsPerWinInStreak = command.PointsPerWinInStreak,
            MinWinStreakToActivate = command.MinWinStreakToActivate,
            IsLossStreakEnabled = command.IsLossStreakEnabled,
            PointsPerLossInStreak = command.PointsPerLossInStreak,
            MinLossStreakToActivate = command.MinLossStreakToActivate,
            IsMvpEnabled = command.IsMvpEnabled,
            PointsPerMvp = command.PointsPerMvp
        };

        _context.Leagues.Add(league);
        await _context.SaveChangesAsync();

        // Crear jugadores iniciales
        var players = command.Players.Select(p => new Player
        {
            FirstName = p.FirstName,
            LastName = p.LastName,
            LeagueId = league.Id
        }).ToList();

        _context.Players.AddRange(players);
        await _context.SaveChangesAsync();

        return new CreateLeagueResponse(true, "Liga creada exitosamente", league.Id, league.Slug);        
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }

    private static string GenerateSlug(string name)
    {
        var normalizedString = name.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach(var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if(unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        var withoutDiacritics = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        var slug = Regex.Replace(withoutDiacritics.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');
        
        return slug;
    }
}