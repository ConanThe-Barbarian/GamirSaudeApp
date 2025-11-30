using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamirSaude.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace GamirSaude.Infrastructure.Persistence
{
    public class GamirSaudeDbContext : DbContext
    {
        public GamirSaudeDbContext(DbContextOptions<GamirSaudeDbContext> options) : base(options) { }

        // A ÚNICA tabela que esta API gerencia diretamente
        public DbSet<UsuarioApp> UsuariosApp { get; set; }

        // Todo o resto (Agendas, Médicos, etc.) agora vem via HTTP da API SMGH,
        // então não existem DbSets para eles aqui.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário (ex: schemas, índices)
            // modelBuilder.Entity<UsuarioApp>().HasIndex(u => u.Cpf).IsUnique();
        }
    }
}