using GamirSaude.Domain.Interfaces;
using GamirSaude.Infrastructure.Services;// <-- NOVA LINHA
using GamirSaude.Infrastructure.Persistence;
using GamirSaude.Infrastructure.Repositories;               // <-- NOVA LINHA
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----- INÍCIO DO NOSSO CÓDIGO -----

// 1. Ler a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar o DbContext no sistema de injeção de dependência
builder.Services.AddDbContext<GamirSaudeDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Registrar o nosso repositório
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>(); // <-- NOVA LINHA

// ----- FIM DO NOSSO CÓDIGO -----
builder.Services.AddScoped<IEmailService, EmailService>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();