using GamirSaude.Application.DTOs;
using GamirSaude.Domain.Entities;
using GamirSaude.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // Para DbTransaction (apesar de usarmos ADO.NET puro)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace GamirSaude.API.Controllers
{
    // [Authorize] // <-- Em produção, descomente para exigir Token JWT
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        // ============================================================
        // 1. LISTA DE ESPECIALIDADES
        // ============================================================
        [HttpGet("especialidades")]
        public IActionResult GetEspecialidades()
        {
            // MOCK: Simulando retorno do Legado
            var lista = new List<EspecialidadeDto>
            {
                new EspecialidadeDto { Nome = "Cardiologia", Icone = "heart_pulse.png", Cor = "#FFEBEE" },
                new EspecialidadeDto { Nome = "Dermatologia", Icone = "skin.png", Cor = "#E3F2FD" },
                new EspecialidadeDto { Nome = "Ortopedia", Icone = "bone.png", Cor = "#F3E5F5" },
                new EspecialidadeDto { Nome = "Clínica Geral", Icone = "stethoscope.png", Cor = "#E8F5E9" },
                new EspecialidadeDto { Nome = "Pediatria", Icone = "baby_face.png", Cor = "#FFFDE7" }
            };

            return Ok(lista);
        }

        // ============================================================
        // 2. LISTA DE MÉDICOS (Filtro por Especialidade)
        // ============================================================
        [HttpGet("medicos/{especialidade}")]
        public IActionResult GetMedicos(string especialidade)
        {
            // MOCK: Simulando banco de médicos do SMGH
            var todosMedicos = new List<MedicoDto>
            {
                new MedicoDto { Id = 1, Nome = "Dr. Estranho", Especialidade = "Cardiologia", Crm = "12345-RJ", ValorConsulta = 350.00, Foto = "doctor_male.png", Descricao = "Especialista em cirurgias de alto risco." },
                new MedicoDto { Id = 2, Nome = "Dra. Grey", Especialidade = "Cardiologia", Crm = "54321-RJ", ValorConsulta = 300.00, Foto = "doctor_female.png", Descricao = "Foco em prevenção e reabilitação." },
                new MedicoDto { Id = 3, Nome = "Dr. House", Especialidade = "Clínica Geral", Crm = "99999-RJ", ValorConsulta = 500.00, Foto = "doctor_male.png", Descricao = "Diagnósticos complexos." },
                new MedicoDto { Id = 4, Nome = "Dra. Quinn", Especialidade = "Dermatologia", Crm = "11111-RJ", ValorConsulta = 250.00, Foto = "doctor_female.png", Descricao = "Estética e patologias da pele." }
            };

            // Filtra pela especialidade pedida (ou retorna tudo se for "Todas")
            var filtrados = especialidade.ToLower() == "todas"
                ? todosMedicos
                : todosMedicos.Where(m => m.Especialidade.ToLower() == especialidade.ToLower()).ToList();

            return Ok(filtrados);
        }

        // ============================================================
        // 3. DIAS DISPONÍVEIS (Calendário)
        // ============================================================
        [HttpPost("dias-disponiveis")]
        public IActionResult GetDiasDisponiveis([FromBody] DiasDisponiveisRequest request)
        {
            // MOCK: Gera dias aleatórios para o mês solicitado
            var dias = new List<DiaDisponivelDto>();
            int diasNoMes = DateTime.DaysInMonth(request.Ano, request.Mes);

            // Simula que terças e quintas estão livres
            for (int i = 1; i <= diasNoMes; i++)
            {
                var data = new DateTime(request.Ano, request.Mes, i);

                // Lógica fake: Disponível se for dia par e não for domingo
                bool isDisponivel = (i % 2 == 0) && data.DayOfWeek != DayOfWeek.Sunday;

                if (isDisponivel)
                {
                    dias.Add(new DiaDisponivelDto { Data = data, Disponivel = true });
                }
            }

            return Ok(dias);
        }

        // ============================================================
        // 4. HORÁRIOS DISPONÍVEIS
        // ============================================================
        [HttpPost("horarios-disponiveis")]
        public IActionResult GetHorariosDisponiveis([FromBody] HorariosDisponiveisRequest request)
        {
            // MOCK: Retorna horários fixos
            var horarios = new List<string> { "09:00", "09:30", "10:00", "11:30", "14:00", "15:30", "16:00" };
            return Ok(horarios);
        }

        // ============================================================
        // 5. CONFIRMAR AGENDAMENTO (Ação Final)
        // ============================================================
        [HttpPost] // O App manda para /api/agendamento
        public IActionResult AgendarConsulta([FromBody] AgendamentoRequest request)
        {
            // 1. Validação
            if (request.IdPacienteLegado <= 0)
            {
                return BadRequest("Paciente não identificado no sistema legado (ID inválido).");
            }

            if (request.IdMedico <= 0)
            {
                return BadRequest("Médico inválido.");
            }

            // 2. Simulação de Processamento (Facade)
            // Aqui a GamirAPI chamaria a API do SMGH: _smghService.Agendar(...)
            // Vamos apenas logar no console para você ver acontecendo.

            Console.WriteLine($"\n[AGENDAMENTO MOCK] ===============================");
            Console.WriteLine($"[PACIENTE] ID Legado: {request.IdPacienteLegado}");
            Console.WriteLine($"[MEDICO] ID: {request.IdMedico}");
            Console.WriteLine($"[DATA] {request.DataHorario}");
            if (request.IdProcedimento.HasValue)
                Console.WriteLine($"[PROCEDIMENTO] ID: {request.IdProcedimento}");
            Console.WriteLine($"[OBS] {request.Observacao}");
            Console.WriteLine($"==================================================\n");

            // 3. Retorno de Sucesso
            return Ok(new { message = "Agendamento realizado com sucesso!", protocolo = Guid.NewGuid() });
        }

        // ============================================================
        // 6. HISTÓRICO DE AGENDAMENTOS
        // ============================================================
        [HttpGet("historico/{idPaciente}")]
        public IActionResult GetHistorico(int idPaciente)
        {
            // MOCK: Simulando busca no banco do legado
            var lista = new List<AgendamentoHistoricoDto>
            {
                // 1. O Agendamento "Recente" (Futuro)
                new AgendamentoHistoricoDto {
                    Id = 101,
                    NomePrestador = "Dr. Estranho",
                    Especialidade = "Cardiologia",
                    Procedimento = "CONSULTA MÉDICA",
                    DataHoraMarcada = DateTime.Now.AddDays(2).AddHours(4), // Daqui a 2 dias
                    Valor = 350.00m,
                    Status = "Confirmado",
                    Desativado = false // Aba Pendentes
                },
                
                // 2. Um Exame Pendente
                new AgendamentoHistoricoDto {
                    Id = 102,
                    NomePrestador = "Laboratório Central",
                    Especialidade = "Diagnóstico",
                    Procedimento = "HEMOGRAMA COMPLETO",
                    DataHoraMarcada = DateTime.Now.AddDays(5).AddHours(1),
                    Valor = 80.00m,
                    Status = "Aguardando",
                    Desativado = false // Aba Pendentes
                },

                // 3. Histórico Passado (Fechado)
                new AgendamentoHistoricoDto {
                    Id = 99,
                    NomePrestador = "Dra. Grey",
                    Especialidade = "Cardiologia",
                    Procedimento = "CONSULTA ROTINA",
                    DataHoraMarcada = DateTime.Now.AddDays(-20),
                    Valor = 300.00m,
                    Status = "Realizado",
                    Desativado = true // Aba Fechados
                },
                 new AgendamentoHistoricoDto {
                    Id = 98,
                    NomePrestador = "Dr. House",
                    Especialidade = "Clínica Geral",
                    Procedimento = "CHECKUP",
                    DataHoraMarcada = DateTime.Now.AddMonths(-1),
                    Valor = 500.00m,
                    Status = "Cancelado",
                    Desativado = true // Aba Fechados
                }
            };

            return Ok(lista);
        }
    }
}