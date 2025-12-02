using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace GamirSaudeApp.Models
{
    public class DashboardBannerItem
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string DetalhePrincipal { get; set; } // O texto grande (Data ou Preço)
        public string Icone { get; set; }
        public Color CorFundo { get; set; }
        public bool IsAgendamento { get; set; } // Define se mostra o botão "Ver Detalhes"
    }
}