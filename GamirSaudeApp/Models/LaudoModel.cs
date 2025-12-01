using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace GamirSaudeApp.Models
{
    public class LaudoModel
    {
        public int Id { get; set; }
        public string NomeExame { get; set; }
        public string NomeMedico { get; set; }
        public string Especialidade { get; set; }
        public DateTime DataExame { get; set; }
        public string Status { get; set; }
        public string UrlPdf { get; set; }

        // Helpers Visuais
        public bool IsDisponivel => Status == "Disponível";
        public string CorStatus => Status == "Disponível" ? "#0098DA" : "#F2994A"; // Azul ou Laranja
    }
}