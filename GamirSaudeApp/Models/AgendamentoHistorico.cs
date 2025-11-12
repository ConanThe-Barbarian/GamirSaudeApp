using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// GamirSaudeApp/Models/AgendamentoHistorico.cs

using System;

namespace GamirSaudeApp.Models
{
    // Modelo usado para exibir o histórico de agendamentos
    public class AgendamentoHistorico
    {
        public int IdAgenda { get; set; }
        public int IdAtendimento { get; set; }
        public DateTime DataHoraMarcada { get; set; }
        public string NomePrestador { get; set; }
        public string Especialidade { get; set; }
        public string Procedimento { get; set; }
        public bool Desativado { get; set; }

        // Propriedade para controle da UI
        public bool PodeCancelar => !Desativado && DataHoraMarcada > DateTime.Now.AddHours(2); // Ex: Só pode cancelar com 2h de antecedência
        public string StatusDisplay => Desativado ? "Cancelado" : (DataHoraMarcada < DateTime.Now ? "Realizado" : "Ativo");
    }
}