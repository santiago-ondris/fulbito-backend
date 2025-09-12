using Fulbito.Core.Common.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Fulbito.Core.Features.Authentication.Login;

public class LoginHandler
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public LoginHandler(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public async Task<LoginResponse> Handle(LoginCommand command)
    {
        // Buscar usuario
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null)
        {
            return new LoginResponse(false, "Credenciales inválidas");
        }

        // Verificar contraseña
        var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, false);
        if (!result.Succeeded)
        {
            return new LoginResponse(false, "Credenciales inválidas");
        }

        // Generar JWT token
        var token = GenerateJwtToken(user);
        return new LoginResponse(true, "Login exitoso", token);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}