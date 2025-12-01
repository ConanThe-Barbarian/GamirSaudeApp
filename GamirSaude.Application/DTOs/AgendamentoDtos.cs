using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    // ============================================================
    // OBJETOS DE LEITURA (O que a API devolve)
    // ============================================================

    public class EspecialidadeDto
    {
        public string Nome { get; set; }
        public string Icone { get; set; } // Ex: "cardio.png"
        public string Cor { get; set; }   // Ex: "#FFEBEE"
    }

    public class MedicoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string Crm { get; set; }
        public string Foto { get; set; }
        public double ValorConsulta { get; set; }
        public string Descricao { get; set; }
    }

    public class DiaDisponivelDto
    {
        public DateTime Data { get; set; }
        public bool Disponivel { get; set; }
    }

    // ============================================================
    // REQUISIÇÕES (O que o App envia para buscar dados)
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

    // ============================================================
    // AÇÃO FINAL (Confirmar o Agendamento)
    // ============================================================

    public class AgendamentoRequest
    {
        public int IdPacienteLegado { get; set; } // O ID 88922 que salvamos na verificação
        public int IdMedico { get; set; }
        public int? IdProcedimento { get; set; }
        public DateTime DataHorario { get; set; }
        public string Observacao { get; set; }
        public int? Minutos { get; set; }
    }

    public class AgendamentoHistoricoDto
    {
        public int Id { get; set; }
        public string NomePrestador { get; set; }
        public string Especialidade { get; set; }
        public string Procedimento { get; set; }
        public DateTime DataHoraMarcada { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; } // "Agendado", "Realizado", "Cancelado"
        public bool Desativado { get; set; } // Define se vai para aba "Fechados"
    }
}