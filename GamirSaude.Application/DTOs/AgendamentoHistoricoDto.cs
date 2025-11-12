using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamirSaude.Domain.Entities;

namespace GamirSaude.Application.DTOs
{
    // DTO usado para retornar o histórico de agendamentos para a API
    public class AgendamentoHistoricoDto
    {
        public int IdAgenda { get; set; }
        public int IdAtendimento { get; set; }
        public DateTime DataHoraMarcada { get; set; }
        public string? NomePrestador { get; set; }
        public string? Especialidade { get; set; }
        public string? Procedimento { get; set; }
        public bool Desativado { get; set; }

        // Propriedades calculadas podem ser úteis, mas geralmente são feitas no Frontend
        public string StatusDisplay => Desativado ? "Cancelado" : (DataHoraMarcada < DateTime.Now ? "Realizado" : "Ativo");
    }
}