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
        public string NomeExame { get; set; } // Ex: Raio X de Tórax
        public DateTime DataExame { get; set; }
        public string Status { get; set; } // "Liberado", "Pendente", "Cancelado"
        public string UrlPdf { get; set; } // Link simulado
        public string NomeMedico { get; set; }
        public string Especialidade { get; set; }

        // --- Propriedades Visuais ---

        public bool IsLiberado => Status == "Liberado";

        public Color StatusColor => Status switch
        {
            "Liberado" => Color.FromArgb("#6FCF97"), // Verde Sucesso
            "Pendente" => Color.FromArgb("#F2994A"), // Laranja
            "Cancelado" => Color.FromArgb("#EB5757"), // Vermelho
            _ => Colors.Gray
        };

        public string TextoBotao => IsLiberado ? "Visualizar Laudo" : "Aguardando...";
        public bool BotaoHabilitado => IsLiberado;

        public double OpacidadeBotao => IsLiberado ? 1.0 : 0.5;
    }
}