using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Fulbito.Core.Database;
using Fulbito.Core.Common.Entities;
using Fulbito.Core.Features.Authentication;
using Fulbito.Core.Features.CreateLeague;
using Fulbito.Core.Features.ViewLeague;
using Fulbito.Core.Features.AddMatch;
using Fulbito.Core.Features.ManageLeague;
using Fulbito.Core.Features.AdminLeague;
using Fulbito.Core.Features.ViewMatchups;
using Fulbito.Core.Common.Configuration;
using Fulbito.Core.Features.UpdatePlayerImage;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Habilitar variables de entorno para que pisen JSON
builder.Configuration.AddEnvironmentVariables();

// Services base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

string? dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrWhiteSpace(dbUrl))
{
    // Ej: postgres://user:pass@host:port/db
    var uri = new Uri(dbUrl);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";

    var npgsqlCs = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = username,
        Password = password,
        SslMode = SslMode.Require
    }.ToString();

    connectionString = npgsqlCs;
}
else
{
    // Local/dev: toma del appsettings.Development.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection para entorno local.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ---- Cloudinary ----
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<CloudinaryDotNet.Cloudinary>(provider =>
{
    var config = builder.Configuration.GetSection("Cloudinary");
    var account = new CloudinaryDotNet.Account(
        config["CloudName"],
        config["ApiKey"],
        config["ApiSecret"]
    );
    return new CloudinaryDotNet.Cloudinary(account);
});

// ---- Identity ----
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ---- JWT ----
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

// ---- CORS ----
// FRONTEND_ORIGIN en env para prod; default a Vite local en dev
var frontendOrigin = Environment.GetEnvironmentVariable("FRONTEND_ORIGIN") ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Features (los tuyos)
builder.Services.AddAuthenticationFeatures();
builder.Services.AddCreateLeagueFeatures();
builder.Services.AddViewLeagueFeatures();
builder.Services.AddMatchFeatures();
builder.Services.AddManageLeagueFeatures();
builder.Services.AddAdminLeagueFeatures();
builder.Services.AddViewMatchupsFeatures();
builder.Services.AddUpdatePlayerImageFeatures();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error aplicando migraciones en el arranque");
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapAuthenticationEndpoints();
app.MapCreateLeagueEndpoints();
app.MapViewLeagueEndpoints();
app.MapAddMatchEndpoints();
app.MapManageLeagueEndpoints();
app.MapAdminLeagueEndpoints();
app.MapViewMatchupsEndpoints();

app.Run();
