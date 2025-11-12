using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("vw_Cad_PlanoConvenioProcedimento")]
    public class PlanoConvenioProcedimentoView
    {
        [Key]
        public int idPlanoConvenioProcedimento { get; set; }
        public int? idProcedimentoTussNomenclatura { get; set; }
        public string? PTNDescricaoTuss { get; set; }
        [Column("PTVValProcedimento", TypeName = "decimal(18, 2)")]
        public decimal? Valor { get; set; }
        public int? idPlanoConvenio { get; set; }

        // ===== PROPRIEDADE CRÍTICA ADICIONADA ABAIXO =====
        public string? NomePlanoInterno { get; set; }
    }
}