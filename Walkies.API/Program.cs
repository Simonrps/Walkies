using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Walkies.API.Data;

/// <summary>
/// Entry point of the Walkies API application.
/// Configures services, middleware, authentication and database context
/// before building and running the application.
/// </summary>
/// 
var builder = WebApplication.CreateBuilder(args);

// Database configuration

/// <summary>
/// Registers the ApplicationDbContext with Entity Framework Core using the
/// SQL Server provider and the connection string from appsettings.json.
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication configuration

/// <summary>
/// Configures JWT Bearer authentication using settings from appsettings.json.
/// App API endpoints are secured by default and require a valid JWT token
/// unless explicitly marked with [AllowAnonymous].
/// </summary>
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey!))
    };
});

builder.Services.AddAuthorization();

// Controllers configuration

builder.Services.AddControllers();

// Swagger configuration

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build and middleware configuration

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();