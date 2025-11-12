using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class AgendamentoRequestDto
    {
        public int IdPaciente { get; set; }
        public int IdPrestadorMedico { get; set; }
        public DateTime DataHoraMarcada { get; set; }
        public int IdProcedimento { get; set; }
        public int Minutos { get; set; }
    }
}
