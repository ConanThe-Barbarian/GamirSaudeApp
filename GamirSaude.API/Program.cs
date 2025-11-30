using GamirSaude.Infrastructure.Persistence;
using GamirSaude.Infrastructure.Services;
using GamirSaude.Application.Services; // <--- ONDE ESTÃO AS INTERFACES
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer; // <--- AGORA VAI FUNCIONAR
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Se IEmailService estiver em Domain.Interfaces, descomente:
// using GamirSaude.Domain.Interfaces; 

var builder = WebApplication.CreateBuilder(args);

// 1. BANCO DE DADOS
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GamirSaudeDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. INJEÇÃO DE DEPENDÊNCIA
// Garante que o C# sabe onde estão as classes e interfaces
builder.Services.AddScoped<ITokenService, TokenService>();


// 3. AUTENTICAÇÃO JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();