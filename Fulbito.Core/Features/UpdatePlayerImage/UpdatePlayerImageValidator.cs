using Microsoft.AspNetCore.Http;

namespace Fulbito.Core.Features.UpdatePlayerImage;

public class UpdatePlayerImageValidator
{
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif" };
    private const int MaxFileSizeBytes = 5 * 1024 * 1024;

    public ValidationResult Validate(UpdatePlayerImageCommand command)
    {
        var errors = new List<string>();

        if (command.LeagueId == Guid.Empty)
            errors.Add("ID de liga inválido");

        if (command.PlayerId == Guid.Empty)
            errors.Add("ID de jugador inválido");

        if (command.ImageFile == null)
        {
            errors.Add("Imagen requerida");
        }
        else
        {
            ValidateImageFile(command.ImageFile, errors);
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    private void ValidateImageFile(IFormFile file, List<string> errors)
    {
        if (file.Length == 0)
        {
            errors.Add("El archivo está vacío");
            return;
        }

        if (file.Length > MaxFileSizeBytes)
        {
            errors.Add("El archivo es demasiado grande. Máximo 5MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            errors.Add("Formato de archivo no permitido. Use: JPG, JPEG, PNG o GIF");
        }

        if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            errors.Add("Tipo de contenido no válido");
        }
    }
}

public record ValidationResult(bool IsValid, List<string> Errors);