using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fulbito.Core.Common.Entities;
using Fulbito.Core.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Fulbito.Core.Features.UpdatePlayerImage;

public class UpdatePlayerImageHandler
{
    private readonly ApplicationDbContext _context;
    private readonly Cloudinary _cloudinary;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdatePlayerImageHandler(
        ApplicationDbContext context, 
        Cloudinary cloudinary,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _cloudinary = cloudinary;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UpdatePlayerImageResponse> Handle(UpdatePlayerImageCommand command)
    {
        var userId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var league = await _context.Leagues
            .FirstOrDefaultAsync(l => l.Id == command.LeagueId);
        
        if (league == null)
            return new UpdatePlayerImageResponse(false, "Liga no encontrada o no tenés permisos");

        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == command.PlayerId && p.LeagueId == command.LeagueId);
        
        if (player == null)
            return new UpdatePlayerImageResponse(false, "Jugador no encontrado");

        try
        {
            var uploadResult = await UploadImageToCloudinary(command.ImageFile, player);
            
            if (uploadResult.Error != null)
                return new UpdatePlayerImageResponse(false, $"Error al subir imagen: {uploadResult.Error.Message}");

            if (!string.IsNullOrEmpty(player.ImageUrl))
            {
                await DeleteOldImageFromCloudinary(player.ImageUrl);
            }

            player.ImageUrl = uploadResult.SecureUrl.ToString();
            await _context.SaveChangesAsync();

            return new UpdatePlayerImageResponse(true, "Imagen actualizada exitosamente", player.ImageUrl);
        }
        catch (Exception ex)
        {
            return new UpdatePlayerImageResponse(false, $"Error interno: {ex.Message}");
        }
    }

    private async Task<ImageUploadResult> UploadImageToCloudinary(IFormFile file, Player player)
    {
        using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = $"fulbito/players/{player.LeagueId}/{player.Id}",
            Transformation = new Transformation()
                .Width(300).Height(300)
                .Crop("fill")
                .Quality("auto:good"),
            Overwrite = true
        };

        return await _cloudinary.UploadAsync(uploadParams);
    }

    private async Task DeleteOldImageFromCloudinary(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');
            var publicIdWithExtension = string.Join("/", segments.Skip(4));
            var publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);
            var folder = string.Join("/", segments.Skip(4).Take(segments.Length - 5));
            var fullPublicId = $"{folder}/{publicId}";

            await _cloudinary.DestroyAsync(new DeletionParams(fullPublicId));
        }
        catch (Exception)
        {
        }
    }
}