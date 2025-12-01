using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GamirSaudeApp.Models
{
    // ============================================================
    // OBJETOS VISUAIS E DE RETORNO (API -> APP)
    // ============================================================

    public class Especialidade
    {
        public string Nome { get; set; }
        public string Icone { get; set; }
        public string Cor { get; set; }
    }

    public class Medico
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string Crm { get; set; }
        public string Foto { get; set; }
        public double ValorConsulta { get; set; }
        public string Descricao { get; set; }
    }

    public class DiaDisponivel
    {
        public DateTime Data { get; set; }
        public bool Disponivel { get; set; }
    }

    // --- NOVO: HISTÓRICO UNIFICADO AQUI ---
    public class AgendamentoHistorico
    {
        public int Id { get; set; }
        public string NomePrestador { get; set; }
        public string Especialidade { get; set; }
        public string Procedimento { get; set; }
        public DateTime DataHoraMarcada { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; } // "Agendado", "Realizado", "Cancelado"
        public bool Desativado { get; set; } // Define se vai para aba "Fechados"

        // Propriedades Visuais (Helpers)
        public string ValorFormatado => Valor > 0 ? $"R$ {Valor:F2}" : "Grátis";

        public bool IsPendente => !Desativado;
        public bool IsFechado => Desativado;

        public string StatusTextColor => Status == "Cancelado" ? "#EB5757" : "#0098DA";
        public string StatusColor => Status == "Cancelado" ? "#FFEEEE" : "#E3E9FF";
    }

    // ============================================================
    // REQUISIÇÕES (APP -> API)
    // ============================================================

    public class DiasDisponiveisRequest
    {
        public int IdMedico { get; set; }
        public int? IdProcedimento { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
    }

    public class HorariosDisponiveisRequest
    {
        public int IdMedico { get; set; }
        public int? IdProcedimento { get; set; }
        public DateTime Data { get; set; }
    }

    public class AgendamentoRequest
    {
        public int IdPacienteLegado { get; set; }
        public int IdMedico { get; set; }
        public int? IdProcedimento { get; set; }
        public DateTime DataHorario { get; set; }
        public string Observacao { get; set; }
        public int? Minutos { get; set; }
    }
}