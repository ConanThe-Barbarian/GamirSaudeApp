using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class DiasDisponiveisRequestDto
    {
        public int IdMedico { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }       
        public int IdProcedimento { get; set; } // Necessário para Exames E para Horários/Agendamento de Consulta
    }
}