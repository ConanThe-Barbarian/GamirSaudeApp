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
        public GamirSaudeDbContext(DbContextOptions<GamirSaudeDbContext> options) : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Agenda> Agendas { get; set; }
        public DbSet<PrestadorMedico> PrestadoresMedicos { get; set; }
        public DbSet<PacienteAnexo> PacientesAnexos { get; set; }
        public DbSet<Especialidade> Especialidades { get; set; }
        public DbSet<PrestadorMedicoEspecialidade> PrestadoresMedicosEspecialidades { get; set; }
        public DbSet<AgendaWebView> AgendaWebView { get; set; }
        public DbSet<AgendaView> VwCadAgenda { get; set; }
        public DbSet<PlanoConvenioProcedimentoView> VwPlanoConvenioProcedimento { get; set; }
        public DbSet<ProcedimentoTussNomenclatura> ProcedimentosTussNomenclatura { get; set; }
        public DbSet<Cad_UsuarioApp> UsuariosApp { get; set; }
        public DbSet<Cad_PacientePlano> Cad_PacientePlano { get; set; }
        public DbSet<Tab_Atendimento> Tab_Atendimento { get; set; } // Nome Pluralizado

        // Configuração adicional para chaves compostas ou nomes de tabelas/colunas específicos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Se houver nomes específicos que o EF não consegue inferir, configure-os aqui.
            // Exemplo: modelBuilder.Entity<AgendaWebView>().HasNoKey(); // Se for uma view sem chave primária

            // Garante que o EF use os nomes corretos definidos pelos [Table] e [Column]
            base.OnModelCreating(modelBuilder);
        }
    }
}