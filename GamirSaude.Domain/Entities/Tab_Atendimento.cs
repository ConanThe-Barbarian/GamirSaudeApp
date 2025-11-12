using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("Tab_Atendimento")]
    public class Tab_Atendimento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idAtendimento { get; set; }

        public int? idPaciente { get; set; }
        public int? idMedicoExecutante { get; set; }
        public DateTime? DataAtendimento { get; set; }
        public DateTime? DataHoraAgendamento { get; set; } // Campo que usamos para a hora
        public int? idPacientePlano { get; set; }
        public int? idPlanoConvenio { get; set; }
        public int? idConvenio { get; set; }
        public int? idUsuarioInclusao { get; set; }
        public DateTime? DataInclusao { get; set; }
        public int? idEspecialidadeExecutante { get; set; }
        public bool Desativado { get; set; } = false;
        public DateTime? DataCancelamento { get; set; }
        public int? idUsuarioCancelamento { get; set; }
        public int? idAgenda { get; set; } // Ligação com Cad_Agenda

        // Adicione outras colunas se forem necessárias para futuras consultas
    }
}
