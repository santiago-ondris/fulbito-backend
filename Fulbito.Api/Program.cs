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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Identity
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

// JWT Authentication
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite default port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication();

builder.Services.AddAuthenticationFeatures();
builder.Services.AddCreateLeagueFeatures();
builder.Services.AddViewLeagueFeatures();
builder.Services.AddMatchFeatures();
builder.Services.AddManageLeagueFeatures();
builder.Services.AddAdminLeagueFeatures();
builder.Services.AddViewMatchupsFeatures();
builder.Services.AddUpdatePlayerImageFeatures();

var app = builder.Build();

// Configure the pipeline.
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