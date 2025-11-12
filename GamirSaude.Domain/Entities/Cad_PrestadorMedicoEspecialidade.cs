using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_PrestadorMedicoEspecialidade")]
    public class PrestadorMedicoEspecialidade
    {
        [Key]
        [Column("idPrestadorMedicoEspecialidade")]
        public int Id { get; set; }

        [Column("idPrestadorMedico")]
        public int IdPrestadorMedico { get; set; }

        [Column("idCbos")] // idCbos representa a especialidade
        public int IdEspecialidade { get; set; }
    }
}