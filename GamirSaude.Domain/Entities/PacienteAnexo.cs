using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_PacienteAnexo")]
    public class PacienteAnexo
    {
        // Chave primária da tabela de anexos
        [Key]
        public int IdPacienteAnexo { get; set; }

        // Chave estrangeira que liga o anexo a um paciente
        public int? IdPaciente { get; set; }

        // Data e hora em que o anexo foi adicionado
        public DateTime? DataHora { get; set; }

        // Descrição do anexo (ex: "Raio-X do Tórax")
        public string? Descricao { get; set; }

        // Coluna que armazena o caminho ou identificador do arquivo na nuvem (AWS) 
        public string? RespostaAWS { get; set; }
    }
}