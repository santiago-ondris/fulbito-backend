using System.Security.Claims;
using Fulbito.Core.Common.Entities;
using Fulbito.Core.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Fulbito.Core.Features.ManageLeague;

public class ManageLeagueHandler
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ManageLeagueHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    // ========== ADD PLAYER ==========
    public async Task<AddPlayerResponse> Handle(AddPlayerCommand command)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return new AddPlayerResponse(false, "Usuario no autenticado");
        }
        command.UserId = userId.Value;

        try
        {
            var player = new Player
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                LeagueId = command.LeagueId
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return new AddPlayerResponse(true, "Jugador agregado exitosamente", player.Id);
        }
        catch (Exception ex)
        {
            return new AddPlayerResponse(false, $"Error al agregar jugador: {ex.Message}");
        }
    }

    // ========== EDIT PLAYER ==========
    public async Task<EditPlayerResponse> Handle(EditPlayerCommand command)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return new EditPlayerResponse(false, "Usuario no autenticado");
        }
        command.UserId = userId.Value;

        try
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == command.PlayerId && p.LeagueId == command.LeagueId);

            if (player == null)
            {
                return new EditPlayerResponse(false, "Jugador no encontrado");
            }

            // Actualizar datos
            player.FirstName = command.FirstName;
            player.LastName = command.LastName;
            player.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new EditPlayerResponse(true, "Jugador actualizado exitosamente");
        }
        catch (Exception ex)
        {
            return new EditPlayerResponse(false, $"Error al actualizar jugador: {ex.Message}");
        }
    }

    // ========== DELETE PLAYER ==========
    public async Task<DeletePlayerResponse> Handle(DeletePlayerCommand command)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return new DeletePlayerResponse(false, "Usuario no autenticado");
        }
        command.UserId = userId.Value;

        try
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.Id == command.PlayerId && p.LeagueId == command.LeagueId);

            if (player == null)
            {
                return new DeletePlayerResponse(false, "Jugador no encontrado");
            }

            // Double-check: verificar que no tenga partidos (aunque el validator ya lo hizo)
            var hasMatches = await _context.PlayerMatches
                .AnyAsync(pm => pm.PlayerId == command.PlayerId);

            if (hasMatches)
            {
                return new DeletePlayerResponse(false, 
                    "No se puede eliminar un jugador que ya participó en partidos. Para mantener la integridad de las estadísticas históricas, los jugadores con partidos jugados no pueden ser eliminados.");
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return new DeletePlayerResponse(true, "Jugador eliminado exitosamente");
        }
        catch (Exception ex)
        {
            return new DeletePlayerResponse(false, $"Error al eliminar jugador: {ex.Message}");
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }
}