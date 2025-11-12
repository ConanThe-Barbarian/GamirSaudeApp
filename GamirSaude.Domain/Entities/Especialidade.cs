using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_ANSCbos")]
    public class Especialidade
    {
        [Key]
        [Column("idcbos")] // Corrigido para corresponder à imagem
        public int Id { get; set; }

        [Column("EspecialidadeCbos")] // Corrigido para corresponder à imagem
        public string? Nome { get; set; }

        [JsonPropertyName("idProcedimento")]
        public int IdProcedimento { get; set; }
    }
}