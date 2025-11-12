using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Models
{
    public class HorariosDisponiveisRequest
    {
        public int IdMedico { get; set; }
        public int IdProcedimento { get; set; }
        public DateTime Data { get; set; }
    }
}