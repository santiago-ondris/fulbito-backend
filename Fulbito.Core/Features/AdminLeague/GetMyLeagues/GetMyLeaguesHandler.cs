using System.Security.Claims;
using Fulbito.Core.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.AdminLeague.GetMyLeagues;

public class GetMyLeaguesHandler
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyLeaguesHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetMyLeaguesResponse?> Handle(GetMyLeaguesQuery query)
    {
        // Obtener UserId del JWT
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return null;
        }

        var leagues = await _context.Leagues
            .Where(l => l.UserId == userId)
            .Select(l => new MyLeagueSummary
            {
                Id = l.Id,
                Name = l.Name,
                Slug = l.Slug,
                PlayersPerTeam = l.PlayersPerTeam,
                TotalPlayers = l.Players.Count(),
                TotalMatches = l.Matches.Count(),
                CreatedAt = l.CreatedAt,
                LastMatchDate = l.Matches.OrderByDescending(m => m.MatchDate)
                                        .Select(m => m.MatchDate)
                                        .FirstOrDefault()
            })
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return new GetMyLeaguesResponse { Leagues = leagues };
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }
}