using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    // Model específico para consulta, SEM IdProcedimento
    public class DiasDisponiveisConsultaRequestDto
    {        
        public int IdMedico { get; set; }      
        public int Mes { get; set; }    
        public int Ano { get; set; }
    }
}