using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Models
{
    public class AgendamentoHistorico
    {
        // Propriedades originais (Mantidas)
        public int IdAgenda { get; set; }
        public int IdAtendimento { get; set; }
        public DateTime DataHoraMarcada { get; set; }
        public string NomePrestador { get; set; } // Vamos usar este no lugar de "Medico"
        public string Especialidade { get; set; }
        public string Procedimento { get; set; }
        public bool Desativado { get; set; }
        public decimal Valor { get; set; }

        // --- NOVAS PROPRIEDADES VISUAIS (Para a tela nova) ---

        // Formata o dinheiro (R$ 300,00)
        public string ValorFormatado => Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

        // Decide o título baseado no nome do procedimento
        public string TituloValor
        {
            get
            {
                if (string.IsNullOrEmpty(Procedimento)) return "Valor:";

                // Lógica simples: se tiver "CONSULTA" no nome, é consulta. Senão, é exame.
                return Procedimento.ToUpper().Contains("CONSULTA") ? "Valor da Consulta:" : "Valor do Exame:";
            }
        }

        // Determina o texto do status baseado nas regras de negócio
        public string Status => Desativado ? "Cancelado" :
                               (DataHoraMarcada < DateTime.Now ? "Atendido" : "Confirmado");

        // Define se vai para a aba "Pendentes" ou "Fechados"
        public bool IsPendente => Status == "Agendado" || Status == "Confirmado";
        public bool IsFechado => !IsPendente;

        // Define a cor do badge (Verde, Azul, Vermelho...)
        public Color StatusColor => Status switch
        {
            "Confirmado" => Color.FromArgb("#6FCF97"), // Verde
            "Agendado" => Color.FromArgb("#D6E4FF"),   // Azul claro
            "Cancelado" => Color.FromArgb("#EB5757"),  // Vermelho
            "Atendido" => Color.FromArgb("#E0E0E0"),   // Cinza
            _ => Colors.Gray
        };

        // Define a cor do texto do badge
        public Color StatusTextColor => Status switch
        {
            "Confirmado" => Colors.White,
            "Agendado" => Color.FromArgb("#0098DA"),
            "Cancelado" => Colors.White,
            "Atendido" => Colors.Black,
            _ => Colors.Black
        };
    }
}