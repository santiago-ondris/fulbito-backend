using Microsoft.AspNetCore.Identity;
using Fulbito.Core.Common.Entities;

namespace Fulbito.Core.Features.Authentication.Register;

public class RegisterHandler
{
    private readonly UserManager<User> _userManager;

    public RegisterHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand command)
    {
        // Verificar si el usuario ya existe
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
        {
            return new RegisterResponse(false, "El email ya está registrado");
        }

        // Crear nuevo usuario
        var user = new User
        {
            UserName = command.Email,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (result.Succeeded)
        {
            return new RegisterResponse(true, "Usuario registrado exitosamente", user.Id);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return new RegisterResponse(false, $"Error al registrar usuario: {errors}");
    }
}