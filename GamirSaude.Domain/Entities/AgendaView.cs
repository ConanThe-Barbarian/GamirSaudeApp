using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("vw_Cad_Agenda")]
    public class AgendaView
    {
        [Key]
        public int idagenda { get; set; }
        public int? idPaciente { get; set; }
        public int? idProcedimentoTussNomenclatura { get; set; }
        public int? idPrestadorMedico { get; set; }
        public string? PTNDescricaoTuss { get; set; }

        // Adicione outras colunas da vw_Cad_Agenda se precisar no futuro
    }
}