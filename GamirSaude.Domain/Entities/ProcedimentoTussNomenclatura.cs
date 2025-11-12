using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("Tab_ProcedimentoTussNomenclatura")]
    public class ProcedimentoTussNomenclatura
    {
        [Key]
        public int idProcedimentoTussNomenclatura { get; set; }

        public string PTNDescricaoTuss { get; set; }
    }
}
