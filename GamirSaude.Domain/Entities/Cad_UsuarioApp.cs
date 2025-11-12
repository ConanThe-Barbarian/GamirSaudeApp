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
    public class Cad_UsuarioApp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserApp { get; set; }

        [Required]
        [StringLength(14)]
        public string CpfUserApp { get; set; }

        [Required]
        [StringLength(150)]
        public string NomeUserApp { get; set; }

        [StringLength(20)]
        public string? TelUserApp { get; set; }

        [Required]
        [StringLength(100)]
        public string EmailUserApp { get; set; }

        public DateTime? DataNascUserApp { get; set; }

        [Required]
        [StringLength(128)]
        public string SenhaUserApp { get; set; }

        [StringLength(1)]
        public string? SexoUserApp { get; set; }

        [Required]
        public bool ContaVerificada { get; set; } = false; // Valor padrão definido

        public int? idPacienteGamir { get; set; }

        [StringLength(10)]
        public string? CodigoVerificacao { get; set; }
        public DateTime? DataExpiracaoCodigo { get; set; }
    }
}
