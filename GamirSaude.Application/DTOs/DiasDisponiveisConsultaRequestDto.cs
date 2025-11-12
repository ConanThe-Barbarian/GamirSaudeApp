using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    // DTO específico para consulta, SEM IdProcedimento
    public class DiasDisponiveisConsultaRequestDto
    {
        public int IdMedico { get; set; }
        public DateTime Data { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
    }
}