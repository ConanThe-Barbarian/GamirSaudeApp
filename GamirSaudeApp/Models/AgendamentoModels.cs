using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Models
{
    // Modelos visuais usados nas listas e telas
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

    // Requests para a API
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