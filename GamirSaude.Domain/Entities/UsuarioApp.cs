using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_UsuarioApp")]
    public class UsuarioApp
    {
        [Key]
        [Column("IdUserApp")]
        public int Id { get; set; }

        [Column("CpfUserApp")]
        public string Cpf { get; set; } // Login

        [Column("NomeUserApp")]
        public string Nome { get; set; }

        [Column("EmailUserApp")]
        public string Email { get; set; }

        [Column("SenhaUserApp")]
        public string SenhaHash { get; set; } // Senha Criptografada

        [Column("TelUserApp")]
        public string? Telefone { get; set; }

        [Column("DataNascUserApp")]
        public DateTime? DataNascimento { get; set; }

        [Column("SexoUserApp")]
        public string? Sexo { get; set; } // Char no banco, string aqui facilita

        // --- NOVOS CAMPOS (Upgrade) ---

        [Column("FotoPerfil")]
        public string? FotoPerfilBase64 { get; set; }

        [Column("ContaVerificada")]
        public bool ContaVerificada { get; set; }

        [Column("idPacienteGamir")]
        public int? IdPacienteLegado { get; set; } // A Ponte para o sistema antigo

        [Column("CodigoVerificacao")]
        public string? CodigoVerificacao { get; set; }

        [Column("DataExpiracaoCodigo")]
        public DateTime? DataExpiracaoCodigo { get; set; }
    }
}