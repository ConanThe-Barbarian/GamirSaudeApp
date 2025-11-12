using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_PacientePlano")]
    public class Cad_PacientePlano
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idPacientePlano { get; set; }

        public int? idPaciente { get; set; }
        public int? idPlanoConvenio { get; set; }
        public int? idPlanoConvenioTipo { get; set; }
        public DateTime? DataAtivacaoPlano { get; set; }
        public string? Matricula { get; set; }

        [Column("Titular")]
        public bool Titular { get; set; } = false;

        [Column("Desativado")]
        public bool Desativado { get; set; } = false;

        // Adicione outras colunas essenciais se necessário, mas o EF Core pode ignorar as não mapeadas.
        // As colunas acima são as mínimas para o nosso fluxo.
    }
}
