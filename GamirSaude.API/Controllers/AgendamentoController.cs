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

namespace GamirSaude.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        private readonly GamirSaudeDbContext _context;

        public AgendamentoController(GamirSaudeDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------------
        // ENDPOINT 1: OBTER ESPECIALIDADES (Com ID de Procedimento Dinâmico)
        // -----------------------------------------------------------
        [HttpGet("especialidades")]
        public async Task<IActionResult> GetEspecialidades()
        {
            // Filtro Consulta: AtendeComAgendaWeb = 1 E AtendeComAgendaWebExame = 0
            var todosOsRegistos = await _context.AgendaWebView
                .Where(v => v.AtendeComAgendaWeb == true && v.AtendeComAgendaWebExame == false) // FILTRO CONSULTA
                .Select(v => new {
                    Id = v.idPrestadorMedicoEspecialidade,
                    Nome = v.EspecialidadeCBOS,
                    IdProcedimento = v.idProcedimentoTussNomenclatura
                })
                .ToListAsync();

            var especialidadesUnicas = todosOsRegistos
                .GroupBy(r => r.Nome)
                .Select(g => g.First())
                .OrderBy(r => r.Nome)
                .Select(r => new Especialidade { Id = r.Id, Nome = r.Nome, IdProcedimento = r.IdProcedimento ?? 0 })
                .ToList();

            return Ok(especialidadesUnicas);
        }
        // -----------------------------------------------------------
        // ENDPOINT 2: OBTER MÉDICOS
        // -----------------------------------------------------------
        [HttpGet("medicos/{especialidade}")]
        public async Task<IActionResult> GetMedicosPorEspecialidade(string especialidade)
        {
            // Filtro Consulta: AtendeComAgendaWeb = 1 E AtendeComAgendaWebExame = 0
            // E (RIGOROSO) Pelo menos um dia da semana tem flag=1 E horários preenchidos
            var medicos = await _context.AgendaWebView
                .Where(v => v.EspecialidadeCBOS == especialidade &&
                            v.AtendeComAgendaWeb == true &&
                            v.AtendeComAgendaWebExame == false && // FILTRO CONSULTA
                                                                  // --- FILTRO RIGOROSO REAPLICADO ---
                            (
                                (v.Domingo == true && !string.IsNullOrEmpty(v.DomingoHoraInicio) && !string.IsNullOrEmpty(v.DomingoHoraFim)) ||
                                (v.Segunda == true && !string.IsNullOrEmpty(v.SegundaHoraInicio) && !string.IsNullOrEmpty(v.SegundaHoraFim)) ||
                                (v.Terca == true && !string.IsNullOrEmpty(v.TercaHoraInicio) && !string.IsNullOrEmpty(v.TercaHoraFim)) ||
                                (v.Quarta == true && !string.IsNullOrEmpty(v.QuartaHoraInicio) && !string.IsNullOrEmpty(v.QuartaHoraFim)) ||
                                (v.Quinta == true && !string.IsNullOrEmpty(v.QuintaHoraInicio) && !string.IsNullOrEmpty(v.QuintaHoraFim)) ||
                                (v.Sexta == true && !string.IsNullOrEmpty(v.SextaHoraInicio) && !string.IsNullOrEmpty(v.SextaHoraFim)) ||
                                (v.Sabado == true && !string.IsNullOrEmpty(v.SabadoHoraInicio) && !string.IsNullOrEmpty(v.SabadoHoraFim))
                            )
                      // --- FIM DO FILTRO RIGOROSO ---
                      )
                .Select(v => new { IdPrestadorMedico = v.idPrestadorMedico, Nome = v.PrestadorMedico })
                .Distinct() // Garante que cada médico apareça apenas uma vez
                .OrderBy(m => m.Nome)
                .ToListAsync();

            if (medicos == null || !medicos.Any())
            {
                // Mensagem reflete a validação mais rigorosa
                return NotFound("Nenhum médico com agenda web válida encontrada para esta especialidade.");
            }
            return Ok(medicos);
        }
        // -----------------------------------------------------------
        // ENDPOINT 3: CALCULAR DIAS DISPONÍVEIS (Regra de Vagas Máximas)
        // -----------------------------------------------------------
        [HttpPost("dias-disponiveis")]
        public async Task<IActionResult> GetDiasDisponiveisConsulta([FromBody] DiasDisponiveisConsultaRequestDto request) // Usa DTO SEM IdProcedimento
        {
            var diasDisponiveis = new List<object>();
            var hoje = DateTime.Today;
            var agora = DateTime.Now.TimeOfDay;

            // Seleciona APENAS os dados necessários da regra VÁLIDA
            var regraNecessaria = await _context.AgendaWebView.Where(v => v.idPrestadorMedico == request.IdMedico && v.AtendeComAgendaWeb == true && v.AtendeComAgendaWebExame == false && ((v.Domingo == true && !string.IsNullOrEmpty(v.DomingoHoraInicio) && !string.IsNullOrEmpty(v.DomingoHoraFim)) || (v.Segunda == true && !string.IsNullOrEmpty(v.SegundaHoraInicio) && !string.IsNullOrEmpty(v.SegundaHoraFim)) || (v.Terca == true && !string.IsNullOrEmpty(v.TercaHoraInicio) && !string.IsNullOrEmpty(v.TercaHoraFim)) || (v.Quarta == true && !string.IsNullOrEmpty(v.QuartaHoraInicio) && !string.IsNullOrEmpty(v.QuartaHoraFim)) || (v.Quinta == true && !string.IsNullOrEmpty(v.QuintaHoraInicio) && !string.IsNullOrEmpty(v.QuintaHoraFim)) || (v.Sexta == true && !string.IsNullOrEmpty(v.SextaHoraInicio) && !string.IsNullOrEmpty(v.SextaHoraFim)) || (v.Sabado == true && !string.IsNullOrEmpty(v.SabadoHoraInicio) && !string.IsNullOrEmpty(v.SabadoHoraFim)))).Select(v => new { v.Domingo, v.DomingoQuantidadeMaxima, v.DomingoHoraInicio, v.DomingoHoraFim, v.DomingoAlmoco, v.DomingoAlmocoinicio, v.DomingoAlmocoFim, v.Segunda, v.SegundaQuantidadeMaxima, v.SegundaHoraInicio, v.SegundaHoraFim, v.SegundaAlmoco, v.SegundaAlmocoinicio, v.SegundaAlmocoFim, v.Terca, v.TercaQuantidadeMaxima, v.TercaHoraInicio, v.TercaHoraFim, v.TercaAlmoco, v.TercaAlmocoinicio, v.TercaAlmocoFim, v.Quarta, v.QuartaQuantidadeMaxima, v.QuartaHoraInicio, v.QuartaHoraFim, v.QuartaAlmoco, v.QuartaAlmocoinicio, v.QuartaAlmocoFim, v.Quinta, v.QuintaQuantidadeMaxima, v.QuintaHoraInicio, v.QuintaHoraFim, v.QuintaAlmoco, v.QuintaAlmocoinicio, v.QuintaAlmocoFim, v.Sexta, v.SextaQuantidadeMaxima, v.SextaHoraInicio, v.SextaHoraFim, v.SextaAlmoco, v.SextaAlmocoinicio, v.SextaAlmocoFim, v.Sabado, v.SabadoQuantidadeMaxima, v.SabadoHoraInicio, v.SabadoHoraFim, v.SabadoAlmoco, v.SabadoAlmocoinicio, v.SabadoAlmocoFim, v.EscalaAgenda, v.idProcedimentoTussNomenclatura }).FirstOrDefaultAsync(); // Simplificado

            if (regraNecessaria == null) { Debug.WriteLine($"GetDiasDisponiveisConsulta: Nenhuma regra VÁLIDA para Medico={request.IdMedico}"); return Ok(diasDisponiveis); }
            int diasNoMes = DateTime.DaysInMonth(request.Ano, request.Mes);

            for (int dia = 1; dia <= diasNoMes; dia++)
            {
                var dataAtual = new DateTime(request.Ano, request.Mes, dia);
                if (dataAtual < hoje) continue; // Pula dias passados

                bool trabalhaNoDia = false; int maximoVagas = 0;
                string horaInicioStr = null, horaFimStr = null, almocoInicioStr = null, almocoFimStr = null;
                bool temAlmoco = false;
                switch (dataAtual.DayOfWeek) { case DayOfWeek.Sunday: trabalhaNoDia = regraNecessaria.Domingo; maximoVagas = regraNecessaria.DomingoQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.DomingoHoraInicio; horaFimStr = regraNecessaria.DomingoHoraFim; almocoInicioStr = regraNecessaria.DomingoAlmocoinicio; almocoFimStr = regraNecessaria.DomingoAlmocoFim; temAlmoco = regraNecessaria.DomingoAlmoco; break; case DayOfWeek.Monday: trabalhaNoDia = regraNecessaria.Segunda; maximoVagas = regraNecessaria.SegundaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.SegundaHoraInicio; horaFimStr = regraNecessaria.SegundaHoraFim; almocoInicioStr = regraNecessaria.SegundaAlmocoinicio; almocoFimStr = regraNecessaria.SegundaAlmocoFim; temAlmoco = regraNecessaria.SegundaAlmoco; break; case DayOfWeek.Tuesday: trabalhaNoDia = regraNecessaria.Terca; maximoVagas = regraNecessaria.TercaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.TercaHoraInicio; horaFimStr = regraNecessaria.TercaHoraFim; almocoInicioStr = regraNecessaria.TercaAlmocoinicio; almocoFimStr = regraNecessaria.TercaAlmocoFim; temAlmoco = regraNecessaria.TercaAlmoco; break; case DayOfWeek.Wednesday: trabalhaNoDia = regraNecessaria.Quarta; maximoVagas = regraNecessaria.QuartaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.QuartaHoraInicio; horaFimStr = regraNecessaria.QuartaHoraFim; almocoInicioStr = regraNecessaria.QuartaAlmocoinicio; almocoFimStr = regraNecessaria.QuartaAlmocoFim; temAlmoco = regraNecessaria.QuartaAlmoco; break; case DayOfWeek.Thursday: trabalhaNoDia = regraNecessaria.Quinta; maximoVagas = regraNecessaria.QuintaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.QuintaHoraInicio; horaFimStr = regraNecessaria.QuintaHoraFim; almocoInicioStr = regraNecessaria.QuintaAlmocoinicio; almocoFimStr = regraNecessaria.QuintaAlmocoFim; temAlmoco = regraNecessaria.QuintaAlmoco; break; case DayOfWeek.Friday: trabalhaNoDia = regraNecessaria.Sexta; maximoVagas = regraNecessaria.SextaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.SextaHoraInicio; horaFimStr = regraNecessaria.SextaHoraFim; almocoInicioStr = regraNecessaria.SextaAlmocoinicio; almocoFimStr = regraNecessaria.SextaAlmocoFim; temAlmoco = regraNecessaria.SextaAlmoco; break; case DayOfWeek.Saturday: trabalhaNoDia = regraNecessaria.Sabado; maximoVagas = regraNecessaria.SabadoQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.SabadoHoraInicio; horaFimStr = regraNecessaria.SabadoHoraFim; almocoInicioStr = regraNecessaria.SabadoAlmocoinicio; almocoFimStr = regraNecessaria.SabadoAlmocoFim; temAlmoco = regraNecessaria.SabadoAlmoco; break; } // Simplificado

                if (trabalhaNoDia && maximoVagas > 0)
                {
                    int agendamentosExistentes = await _context.Agendas.CountAsync(a => a.IdPrestadorMedico == request.IdMedico && a.DataHoraMarcada.HasValue && a.DataHoraMarcada.Value.Date == dataAtual.Date && !a.Desativado);
                    int vagasRestantes = maximoVagas - agendamentosExistentes;
                    if (vagasRestantes <= 0) continue; // Pula se lotado

                    if (dataAtual == hoje)
                    {
                        // Chama o método auxiliar
                        bool temHorarioFuturo = await TemHorarioFuturoDisponivel(
                            request.IdMedico,
                            regraNecessaria.idProcedimentoTussNomenclatura ?? 0, // IdProcedimento de Consulta da regra
                            hoje,
                            regraNecessaria.EscalaAgenda ?? 30,
                            horaInicioStr, horaFimStr, almocoInicioStr, almocoFimStr, temAlmoco,
                            agora);
                        if (!temHorarioFuturo) continue; // Pula hoje se não houver slots futuros
                    }

                    if (vagasRestantes > 5) { diasDisponiveis.Add(new { Dia = dia, Status = "Disponivel" }); }
                    else { diasDisponiveis.Add(new { Dia = dia, Status = "PoucasVagas" }); }
                }
            }
            return Ok(diasDisponiveis);
        }
        // -----------------------------------------------------------
        // ENDPOINT 4: CALCULAR HORÁRIOS DISPONÍVEIS (Slots Dinâmicos - CORRIGIDO PARA CANCELAMENTO)
        // -----------------------------------------------------------
        [HttpPost("horarios-disponiveis")]
        public async Task<IActionResult> GetHorariosDisponiveis([FromBody] HorariosDisponiveisRequest request) // Usa DTO COM IdProcedimento
        {
            // Seleciona APENAS os dados necessários da regra de CONSULTA
            var regraNecessaria = await _context.AgendaWebView
                .Where(a => a.idPrestadorMedico == request.IdMedico &&
                           a.AtendeComAgendaWeb == true &&
                           a.AtendeComAgendaWebExame == false) // FILTRO CONSULTA
                .Select(a => new {
                    a.DomingoHoraInicio,
                    a.DomingoHoraFim,
                    a.DomingoAlmoco,
                    a.DomingoAlmocoinicio,
                    a.DomingoAlmocoFim,
                    a.SegundaHoraInicio,
                    a.SegundaHoraFim,
                    a.SegundaAlmoco,
                    a.SegundaAlmocoinicio,
                    a.SegundaAlmocoFim,
                    a.TercaHoraInicio,
                    a.TercaHoraFim,
                    a.TercaAlmoco,
                    a.TercaAlmocoinicio,
                    a.TercaAlmocoFim,
                    a.QuartaHoraInicio,
                    a.QuartaHoraFim,
                    a.QuartaAlmoco,
                    a.QuartaAlmocoinicio,
                    a.QuartaAlmocoFim,
                    a.QuintaHoraInicio,
                    a.QuintaHoraFim,
                    a.QuintaAlmoco,
                    a.QuintaAlmocoinicio,
                    a.QuintaAlmocoFim,
                    a.SextaHoraInicio,
                    a.SextaHoraFim,
                    a.SextaAlmoco,
                    a.SextaAlmocoinicio,
                    a.SextaAlmocoFim,
                    a.SabadoHoraInicio,
                    a.SabadoHoraFim,
                    a.SabadoAlmoco,
                    a.SabadoAlmocoinicio,
                    a.SabadoAlmocoFim,
                    a.EscalaAgenda
                })
                .FirstOrDefaultAsync();

            if (regraNecessaria == null)
            {
                Debug.WriteLine($"GetHorariosDisponiveis (Consulta): Nenhuma regra de consulta encontrada para Medico={request.IdMedico}");
                return Ok(new List<string>());
            }

            // Lógica de obtenção de horaInicioStr, horaFimStr, etc., usando o objeto anônimo 'regraNecessaria'
            string horaInicioStr = null, horaFimStr = null, almocoInicioStr = null, almocoFimStr = null;
            bool temAlmoco = false;
            int duracaoMinutos = regraNecessaria.EscalaAgenda ?? 30;

            switch (request.Data.DayOfWeek) { case DayOfWeek.Sunday: horaInicioStr = regraNecessaria.DomingoHoraInicio; horaFimStr = regraNecessaria.DomingoHoraFim; almocoInicioStr = regraNecessaria.DomingoAlmocoinicio; almocoFimStr = regraNecessaria.DomingoAlmocoFim; temAlmoco = regraNecessaria.DomingoAlmoco; break; case DayOfWeek.Monday: horaInicioStr = regraNecessaria.SegundaHoraInicio; horaFimStr = regraNecessaria.SegundaHoraFim; almocoInicioStr = regraNecessaria.SegundaAlmocoinicio; almocoFimStr = regraNecessaria.SegundaAlmocoFim; temAlmoco = regraNecessaria.SegundaAlmoco; break; case DayOfWeek.Tuesday: horaInicioStr = regraNecessaria.TercaHoraInicio; horaFimStr = regraNecessaria.TercaHoraFim; almocoInicioStr = regraNecessaria.TercaAlmocoinicio; almocoFimStr = regraNecessaria.TercaAlmocoFim; temAlmoco = regraNecessaria.TercaAlmoco; break; case DayOfWeek.Wednesday: horaInicioStr = regraNecessaria.QuartaHoraInicio; horaFimStr = regraNecessaria.QuartaHoraFim; almocoInicioStr = regraNecessaria.QuartaAlmocoinicio; almocoFimStr = regraNecessaria.QuartaAlmocoFim; temAlmoco = regraNecessaria.QuartaAlmoco; break; case DayOfWeek.Thursday: horaInicioStr = regraNecessaria.QuintaHoraInicio; horaFimStr = regraNecessaria.QuintaHoraFim; almocoInicioStr = regraNecessaria.QuintaAlmocoinicio; almocoFimStr = regraNecessaria.QuintaAlmocoFim; temAlmoco = regraNecessaria.QuintaAlmoco; break; case DayOfWeek.Friday: horaInicioStr = regraNecessaria.SextaHoraInicio; horaFimStr = regraNecessaria.SextaHoraFim; almocoInicioStr = regraNecessaria.SextaAlmocoinicio; almocoFimStr = regraNecessaria.SextaAlmocoFim; temAlmoco = regraNecessaria.SextaAlmoco; break; case DayOfWeek.Saturday: horaInicioStr = regraNecessaria.SabadoHoraInicio; horaFimStr = regraNecessaria.SabadoHoraFim; almocoInicioStr = regraNecessaria.SabadoAlmocoinicio; almocoFimStr = regraNecessaria.SabadoAlmocoFim; temAlmoco = regraNecessaria.SabadoAlmoco; break; } // Simplificado

            // Validação e Parse dos horários
            if (!TimeSpan.TryParse(horaInicioStr, out TimeSpan inicio) || !TimeSpan.TryParse(horaFimStr, out TimeSpan fim) || duracaoMinutos <= 0 || inicio >= fim) { return Ok(new List<string>()); }
            TimeSpan almocoInicio = TimeSpan.Zero, almocoFim = TimeSpan.Zero; if (temAlmoco && (!TimeSpan.TryParse(almocoInicioStr, out almocoInicio) || !TimeSpan.TryParse(almocoFimStr, out almocoFim) || almocoInicio >= almocoFim)) { temAlmoco = false; }

            // Busca OCUPADOS usando IdProcedimento
            var ocupados = await _context.Agendas.Where(a => a.IdPrestadorMedico == request.IdMedico && a.IdProcedimentoTussNomenclatura == request.IdProcedimento && a.DataHoraMarcada.HasValue && a.DataHoraMarcada.Value.Date == request.Data.Date && !a.Desativado).Select(a => new { Inicio = a.DataHoraMarcada.Value.TimeOfDay, Duracao = a.Minutos ?? duracaoMinutos }).ToListAsync();

            // Geração de slots
            var disponiveis = new List<string>(); TimeSpan horarioAtual = inicio; TimeSpan slotDuration = TimeSpan.FromMinutes(duracaoMinutos); while (horarioAtual.Add(slotDuration) <= fim) { TimeSpan slotFim = horarioAtual.Add(slotDuration); bool isOcupado = false; if (temAlmoco && horarioAtual < almocoFim && slotFim > almocoInicio) { horarioAtual = almocoFim; continue; } foreach (var ocupado in ocupados) { TimeSpan ocupadoFim = ocupado.Inicio.Add(TimeSpan.FromMinutes(ocupado.Duracao)); if (ocupado.Inicio < slotFim && ocupadoFim > horarioAtual) { isOcupado = true; break; } } if (!isOcupado) { disponiveis.Add(horarioAtual.ToString(@"hh\:mm")); } horarioAtual = horarioAtual.Add(slotDuration); }
            return Ok(disponiveis); // Simplificado
        }

        // -----------------------------------------------------------
        // NOVOS ENDPOINTS (FLUXO DE EXAME)
        // -----------------------------------------------------------

        // -----------------------------------------------------------
        // NOVO ENDPOINT 1: Obter Tipos de Exame (JÁ EXISTE NO GAMIRAPISERVICE, MAS FALTA O ENDPOINT)
        // -----------------------------------------------------------

        [HttpGet("exames/tipos")]
        public async Task<IActionResult> GetTiposExame()
        {
            // Filtro Exame: AtendeComAgendaWeb = 0 E AtendeComAgendaWebExame = 1
            var tipos = await _context.AgendaWebView
                .Where(v => v.AtendeComAgendaWeb == false && v.AtendeComAgendaWebExame == true && v.DescricaoAgenda != null) // FILTRO EXAME
                .Select(v => v.DescricaoAgenda)
                .Distinct()
                .OrderBy(nome => nome)
                .Select(nome => new TipoExameDto { Nome = nome })
                .ToListAsync();

            return Ok(tipos);
        }

        // -----------------------------------------------------------
        // NOVO ENDPOINT 2: Obter Exames Específicos (JÁ EXISTE NO GAMIRAPISERVICE, MAS O ENDPOINT ESTÁ DIFERENTE)
        // -----------------------------------------------------------

        [HttpPost("exames/especificos")]
        public async Task<IActionResult> GetExamesEspecificos([FromBody] TipoExameRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.TipoExame))
            {
                return BadRequest("Tipo de exame inválido.");
            }

            // Filtro Exame: AtendeComAgendaWeb = 0 E AtendeComAgendaWebExame = 1 E DescricaoAgenda corresponde
            var exames = await _context.AgendaWebView
                            .Where(v => v.DescricaoAgenda == request.TipoExame &&
                                        v.AtendeComAgendaWeb == false &&
                                        v.AtendeComAgendaWebExame == true &&
                                        v.idProcedimentoTussNomenclatura != null) // Garante que há um ID para o JOIN
                            .Join(
                                _context.ProcedimentosTussNomenclatura, // Tabela com os nomes (ProcedimentoTussNomenclatura.cs)
                                view => view.idProcedimentoTussNomenclatura, // Chave da AgendaWebView
                                proc => proc.idProcedimentoTussNomenclatura, // Chave da Tab_ProcedimentoTussNomenclatura
                                (view, proc) => new Especialidade // Reutiliza DTO/Entidade Especialidade
                                {
                                    Id = view.idProcedimentoTussNomenclatura ?? 0,
                                    Nome = proc.PTNDescricaoTuss, // <-- CORRIGIDO: Busca o nome da tabela correta
                                    IdProcedimento = view.idProcedimentoTussNomenclatura ?? 0
                                })
                            .Distinct()
                            .OrderBy(e => e.Nome)
                            .ToListAsync();

            return Ok(exames);
        }
        // -----------------------------------------------------------
        // NOVO ENDPOINT 3: Obter Médicos por PROCEDIMENTO (para Exames)
        // -----------------------------------------------------------

        [HttpGet("exames/medicos/{idProcedimento}")]
        public async Task<IActionResult> GetMedicosPorProcedimento(int idProcedimento)
        {
            // Filtro Exame: AtendeComAgendaWeb = 0 E AtendeComAgendaWebExame = 1 E IdProcedimento corresponde
            var medicos = await _context.AgendaWebView
               .Where(v => v.idProcedimentoTussNomenclatura == idProcedimento &&
                           v.AtendeComAgendaWeb == false && v.AtendeComAgendaWebExame == true) // FILTRO EXAME
               .Select(v => new { IdPrestadorMedico = v.idPrestadorMedico, Nome = v.PrestadorMedico })
               .Distinct()
               .OrderBy(m => m.Nome)
               .ToListAsync();

            if (medicos == null || !medicos.Any())
            {
                return NotFound("Nenhum médico encontrado para este exame.");
            }
            return Ok(medicos);
        }

        // -----------------------------------------------------------
        // NOVO ENDPOINT 4: Calcular Dias Disponíveis (para Exames)
        // -----------------------------------------------------------
        [HttpPost("exames/dias-disponiveis")]
        public async Task<IActionResult> GetDiasDisponiveisExame([FromBody] DiasDisponiveisRequestDto request)
        {
            var diasDisponiveis = new List<object>();
            var hoje = DateTime.Today;
            var agora = DateTime.Now.TimeOfDay;

            // Filtro Exame: AtendeComAgendaWeb = 0 E AtendeComAgendaWebExame = 1 E IdMedico/IdProcedimento correspondem
            // Seleciona dados necessários para dias E horários (apenas para hoje)
            var regraNecessaria = await _context.AgendaWebView
                .Where(a => a.idPrestadorMedico == request.IdMedico &&
                           a.idProcedimentoTussNomenclatura == request.IdProcedimento &&
                           a.AtendeComAgendaWeb == false && a.AtendeComAgendaWebExame == true) // FILTRO EXAME
                 .Select(v => new { // Projeção
                     v.Domingo,
                     v.DomingoQuantidadeMaxima,
                     v.DomingoHoraInicio,
                     v.DomingoHoraFim,
                     v.DomingoAlmoco,
                     v.DomingoAlmocoinicio,
                     v.DomingoAlmocoFim,
                     v.Segunda,
                     v.SegundaQuantidadeMaxima,
                     v.SegundaHoraInicio,
                     v.SegundaHoraFim,
                     v.SegundaAlmoco,
                     v.SegundaAlmocoinicio,
                     v.SegundaAlmocoFim,
                     v.Terca,
                     v.TercaQuantidadeMaxima,
                     v.TercaHoraInicio,
                     v.TercaHoraFim,
                     v.TercaAlmoco,
                     v.TercaAlmocoinicio,
                     v.TercaAlmocoFim,
                     v.Quarta,
                     v.QuartaQuantidadeMaxima,
                     v.QuartaHoraInicio,
                     v.QuartaHoraFim,
                     v.QuartaAlmoco,
                     v.QuartaAlmocoinicio,
                     v.QuartaAlmocoFim,
                     v.Quinta,
                     v.QuintaQuantidadeMaxima,
                     v.QuintaHoraInicio,
                     v.QuintaHoraFim,
                     v.QuintaAlmoco,
                     v.QuintaAlmocoinicio,
                     v.QuintaAlmocoFim,
                     v.Sexta,
                     v.SextaQuantidadeMaxima,
                     v.SextaHoraInicio,
                     v.SextaHoraFim,
                     v.SextaAlmoco,
                     v.SextaAlmocoinicio,
                     v.SextaAlmocoFim,
                     v.Sabado,
                     v.SabadoQuantidadeMaxima,
                     v.SabadoHoraInicio,
                     v.SabadoHoraFim,
                     v.SabadoAlmoco,
                     v.SabadoAlmocoinicio,
                     v.SabadoAlmocoFim,
                     v.EscalaAgenda
                 })
                .FirstOrDefaultAsync();

            if (regraNecessaria == null)
            {
                Debug.WriteLine($"GetDiasDisponiveisExame: Nenhuma regra de exame encontrada para Medico={request.IdMedico}, Procedimento={request.IdProcedimento}");
                return Ok(diasDisponiveis);
            }

            int diasNoMes = DateTime.DaysInMonth(request.Ano, request.Mes);

            for (int dia = 1; dia <= diasNoMes; dia++)
            {
                var dataAtual = new DateTime(request.Ano, request.Mes, dia);

                // Pula dias passados
                if (dataAtual < hoje) continue;

                bool trabalhaNoDia = false; int maximoVagas = 0;
                string horaInicioStr = null, horaFimStr = null, almocoInicioStr = null, almocoFimStr = null;
                bool temAlmoco = false;

                // Atribui flags E horários necessários para o dia atual
                switch (dataAtual.DayOfWeek) { case DayOfWeek.Sunday: trabalhaNoDia = regraNecessaria.Domingo; maximoVagas = regraNecessaria.DomingoQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.DomingoHoraInicio; horaFimStr = regraNecessaria.DomingoHoraFim; almocoInicioStr = regraNecessaria.DomingoAlmocoinicio; almocoFimStr = regraNecessaria.DomingoAlmocoFim; temAlmoco = regraNecessaria.DomingoAlmoco; break; case DayOfWeek.Monday: trabalhaNoDia = regraNecessaria.Segunda; maximoVagas = regraNecessaria.SegundaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.SegundaHoraInicio; horaFimStr = regraNecessaria.SegundaHoraFim; almocoInicioStr = regraNecessaria.SegundaAlmocoinicio; almocoFimStr = regraNecessaria.SegundaAlmocoFim; temAlmoco = regraNecessaria.SegundaAlmoco; break; case DayOfWeek.Tuesday: trabalhaNoDia = regraNecessaria.Terca; maximoVagas = regraNecessaria.TercaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.TercaHoraInicio; horaFimStr = regraNecessaria.TercaHoraFim; almocoInicioStr = regraNecessaria.TercaAlmocoinicio; almocoFimStr = regraNecessaria.TercaAlmocoFim; temAlmoco = regraNecessaria.TercaAlmoco; break; case DayOfWeek.Wednesday: trabalhaNoDia = regraNecessaria.Quarta; maximoVagas = regraNecessaria.QuartaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.QuartaHoraInicio; horaFimStr = regraNecessaria.QuartaHoraFim; almocoInicioStr = regraNecessaria.QuartaAlmocoinicio; almocoFimStr = regraNecessaria.QuartaAlmocoFim; temAlmoco = regraNecessaria.QuartaAlmoco; break; case DayOfWeek.Thursday: trabalhaNoDia = regraNecessaria.Quinta; maximoVagas = regraNecessaria.QuintaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.QuintaHoraInicio; horaFimStr = regraNecessaria.QuintaHoraFim; almocoInicioStr = regraNecessaria.QuintaAlmocoinicio; almocoFimStr = regraNecessaria.QuintaAlmocoFim; temAlmoco = regraNecessaria.QuintaAlmoco; break; case DayOfWeek.Friday: trabalhaNoDia = regraNecessaria.Sexta; maximoVagas = regraNecessaria.SextaQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.SextaHoraInicio; horaFimStr = regraNecessaria.SextaHoraFim; almocoInicioStr = regraNecessaria.SextaAlmocoinicio; almocoFimStr = regraNecessaria.SextaAlmocoFim; temAlmoco = regraNecessaria.SextaAlmoco; break; case DayOfWeek.Saturday: trabalhaNoDia = regraNecessaria.Sabado; maximoVagas = regraNecessaria.SabadoQuantidadeMaxima ?? 0; horaInicioStr = regraNecessaria.SabadoHoraInicio; horaFimStr = regraNecessaria.SabadoHoraFim; almocoInicioStr = regraNecessaria.SabadoAlmocoinicio; almocoFimStr = regraNecessaria.SabadoAlmocoFim; temAlmoco = regraNecessaria.SabadoAlmoco; break; } // Simplificado

                if (trabalhaNoDia && maximoVagas > 0)
                {
                    // Contagem de agendamentos para aquele EXAME específico
                    int agendamentosExistentes = await _context.Agendas.CountAsync(a => a.IdPrestadorMedico == request.IdMedico && a.IdProcedimentoTussNomenclatura == request.IdProcedimento && a.DataHoraMarcada.HasValue && a.DataHoraMarcada.Value.Date == dataAtual.Date && !a.Desativado);
                    int vagasRestantes = maximoVagas - agendamentosExistentes;

                    if (vagasRestantes <= 0) continue;

                    // --- VERIFICAÇÃO ADICIONAL PARA O DIA ATUAL ---
                    if (dataAtual == hoje)
                    {
                        bool temHorarioFuturo = await TemHorarioFuturoDisponivel(
                            request.IdMedico,
                            request.IdProcedimento, // ID do Exame
                            hoje,
                            regraNecessaria.EscalaAgenda ?? 30,
                            horaInicioStr, horaFimStr, almocoInicioStr, almocoFimStr, temAlmoco,
                            agora);

                        if (!temHorarioFuturo) continue;
                    }
                    // --- FIM DA VERIFICAÇÃO DO DIA ATUAL ---

                    if (vagasRestantes > 5) { diasDisponiveis.Add(new { Dia = dia, Status = "Disponivel" }); }
                    else { diasDisponiveis.Add(new { Dia = dia, Status = "PoucasVagas" }); }
                }
            }
            return Ok(diasDisponiveis);
        }
        // -----------------------------------------------------------
        // NOVO ENDPOINT 5: Calcular Horários Disponíveis (para Exames)
        // -----------------------------------------------------------

        [HttpPost("exames/horarios-disponiveis")]
        public async Task<IActionResult> GetHorariosDisponiveisExame([FromBody] HorariosDisponiveisRequest request)
        {
            // Filtro Exame: AtendeComAgendaWeb = 0 E AtendeComAgendaWebExame = 1 E IdMedico/IdProcedimento correspondem
            var regraAgenda = await _context.AgendaWebView
                .FirstOrDefaultAsync(a => a.idPrestadorMedico == request.IdMedico &&
                                          a.idProcedimentoTussNomenclatura == request.IdProcedimento &&
                                          a.AtendeComAgendaWeb == false && a.AtendeComAgendaWebExame == true); // FILTRO EXAME

            if (regraAgenda == null) return Ok(new List<string>());

            // Lógica de geração de slots idêntica ao de consulta
            string horaInicioStr = null, horaFimStr = null, almocoInicioStr = null, almocoFimStr = null;
            bool temAlmoco = false;
            int duracaoMinutos = regraAgenda.EscalaAgenda ?? 30;
            switch (request.Data.DayOfWeek) { /* ... (mesma lógica do switch) ... */ default: horaInicioStr = regraAgenda.SegundaHoraInicio; horaFimStr = regraAgenda.SegundaHoraFim; almocoInicioStr = regraAgenda.SegundaAlmocoinicio; almocoFimStr = regraAgenda.SegundaAlmocoFim; temAlmoco = regraAgenda.SegundaAlmoco; break; } // Simplificado para brevidade
            if (!TimeSpan.TryParse(horaInicioStr, out TimeSpan inicio) || !TimeSpan.TryParse(horaFimStr, out TimeSpan fim)) return Ok(new List<string>());
            TimeSpan almocoInicio = TimeSpan.Zero, almocoFim = TimeSpan.Zero;
            if (temAlmoco && (!TimeSpan.TryParse(almocoInicioStr, out almocoInicio) || !TimeSpan.TryParse(almocoFimStr, out almocoFim))) temAlmoco = false;

            var ocupados = await _context.Agendas
                .Where(a => a.IdPrestadorMedico == request.IdMedico &&
                            a.IdProcedimentoTussNomenclatura == request.IdProcedimento && // FILTRO DE EXAME
                            a.DataHoraMarcada.HasValue &&
                            a.DataHoraMarcada.Value.Date == request.Data.Date &&
                            !a.Desativado)
                .Select(a => new { Inicio = a.DataHoraMarcada.Value.TimeOfDay, Duracao = a.Minutos ?? duracaoMinutos })
                .ToListAsync();

            var disponiveis = new List<string>();
            TimeSpan horarioAtual = inicio;
            TimeSpan slotDuration = TimeSpan.FromMinutes(duracaoMinutos);
            while (horarioAtual.Add(slotDuration) <= fim)
            {
                TimeSpan slotFim = horarioAtual.Add(slotDuration);
                bool isOcupado = false;
                if (temAlmoco && horarioAtual < almocoFim && slotFim > almocoInicio) { horarioAtual = almocoFim; continue; }
                foreach (var ocupado in ocupados) { if (ocupado.Inicio < slotFim && ocupado.Inicio.Add(TimeSpan.FromMinutes(ocupado.Duracao)) > horarioAtual) { isOcupado = true; break; } }
                if (!isOcupado) { disponiveis.Add(horarioAtual.ToString(@"hh\:mm")); }
                horarioAtual = horarioAtual.Add(slotDuration);
            }
            return Ok(disponiveis);
        }

        // -----------------------------------------------------------
        // ENDPOINT 5: DE AGENDAMENTO (FINAL COM NOVA PROCEDURE)
        // -----------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AgendarConsulta([FromBody] AgendamentoRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Dados do agendamento inválidos.");
            }

            // --- REGRAS DE NEGÓCIO FIXAS ---
            const int ID_PLANO_CONVENIO_PARTICULAR = 33;
            const int ID_CONVENIO_PARTICULAR = 31;
            const int ID_USUARIO_AUTO = 1;

            var connection = _context.Database.GetDbConnection() as SqlConnection;
            if (connection == null)
            {
                return StatusCode(500, "Erro de infraestrutura: Conexão SQL não encontrada.");
            }

            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                // 1. DEFESA DE CONCORRÊNCIA
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT COUNT(*) FROM Cad_Agenda
                        WHERE idPrestadorMedico = @idMedico 
                        AND DataHoraMarcada = @DataHora 
                        AND Desativado = 0";
                    command.Parameters.Add(new SqlParameter("@idMedico", request.IdPrestadorMedico));
                    command.Parameters.Add(new SqlParameter("@DataHora", request.DataHoraMarcada));
                    int agendamentosExistentes = (int)await command.ExecuteScalarAsync();
                    if (agendamentosExistentes > 0)
                    {
                        await transaction.RollbackAsync();
                        return Conflict(new { message = "O horário selecionado não está mais disponível." });
                    }
                }

                // 2. BUSCAR O IDPACIENTEPLANO
                int idPacientePlano = 0;
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        SELECT TOP 1 pp.idPacientePlano 
                        FROM Cad_PacientePlano pp 
                        WHERE pp.idPaciente = @idPaciente 
                        AND pp.idPlanoConvenio = @idPlanoConvenio 
                        ORDER BY pp.idPacientePlano DESC";
                    command.Parameters.Add(new SqlParameter("@idPaciente", request.IdPaciente));
                    command.Parameters.Add(new SqlParameter("@idPlanoConvenio", ID_PLANO_CONVENIO_PARTICULAR));
                    var result = await command.ExecuteScalarAsync();
                    if (result == null || result is DBNull)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest(new { message = "Falha crítica: Plano Particular (33) não encontrado para o paciente." });
                    }
                    idPacientePlano = (int)result;
                }

                // 3. INSERIR NA TAB_ATENDIMENTO
                int idAtendimento = 0;
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO Tab_Atendimento 
                        (DataAtendimento, idMedicoExecutante, idPaciente, idPlanoConvenio, idConvenio, idUsuarioInclusao, idPacientePlano, DataInclusao, idEspecialidadeExecutante, HoraAtendimentoNormal, DataHoraAgendamento)
                        VALUES (@DataAtendimento, @idMedicoExecutante, @idPaciente, @idPlanoConvenio, @idConvenio, @idUsuarioInclusao, @idPacientePlano, GETDATE(), @idEspecialidadeExecutante, 1, @DataHoraAgendamento);
                        SELECT SCOPE_IDENTITY();";
                    command.Parameters.Add(new SqlParameter("@DataAtendimento", request.DataHoraMarcada)); // DateTime COMPLETO
                    command.Parameters.Add(new SqlParameter("@idMedicoExecutante", request.IdPrestadorMedico));
                    command.Parameters.Add(new SqlParameter("@idPaciente", request.IdPaciente));
                    command.Parameters.Add(new SqlParameter("@idPlanoConvenio", ID_PLANO_CONVENIO_PARTICULAR));
                    command.Parameters.Add(new SqlParameter("@idConvenio", ID_CONVENIO_PARTICULAR));
                    command.Parameters.Add(new SqlParameter("@idUsuarioInclusao", ID_USUARIO_AUTO));
                    command.Parameters.Add(new SqlParameter("@idPacientePlano", idPacientePlano));
                    command.Parameters.Add(new SqlParameter("@idEspecialidadeExecutante", request.IdPrestadorMedico));
                    command.Parameters.Add(new SqlParameter("@DataHoraAgendamento", request.DataHoraMarcada));
                    idAtendimento = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // 4. INSERIR NA TAB_ATENDIMENTOSERVICO
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO Tab_AtendimentoServico
                        (idAtendimento, idProcedimentoTussNomenclatura, idProcedimentoTabelaValor, Minutos)
                        VALUES (@idAtendimento, @idProcedimentoTussNomenclatura, @idProcedimentoTussNomenclatura, @Minutos)";
                    command.Parameters.Add(new SqlParameter("@idAtendimento", idAtendimento));
                    command.Parameters.Add(new SqlParameter("@idProcedimentoTussNomenclatura", request.IdProcedimento));
                    command.Parameters.Add(new SqlParameter("@Minutos", request.Minutos));
                    await command.ExecuteNonQueryAsync();
                }

                // 5. CHAMADA DA NOVA PROCEDURE OTIMIZADA
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = "[PROCEDURE_GAMIR_SAUDE_INSERT]"; // Nome da Nova Procedure
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@idAtendimento", idAtendimento));
                    await command.ExecuteNonQueryAsync();
                }

                // 6. SUCESSO: Commit da transação
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(AgendarConsulta), new { id = idAtendimento }, new { idAtendimento = idAtendimento });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"ERRO CRÍTICO NO AGENDAMENTO: {ex.ToString()}");
                // Em produção, logue o erro em um sistema de logging (Serilog, NLog, etc.)
                return StatusCode(500, new { message = "Ocorreu um erro interno ao processar o agendamento.", detail = ex.Message }); // Não expor ex.ToString() em produção
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }

        // -----------------------------------------------------------
        // ENDPOINT 6: CANCELAR AGENDAMENTO (COM LOGS DE DEBUG)
        // -----------------------------------------------------------
        [HttpPost("cancelar")]
        public async Task<IActionResult> CancelarAgendamento([FromBody] CancelamentoRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Dados de cancelamento inválidos.");
            }

            // --- LOG 1: VERIFICAR DADOS RECEBIDOS ---
            System.Diagnostics.Debug.WriteLine($"---> Iniciando Cancelamento para IdAgenda: {request.IdAgenda}, Usuário: {request.IdUsuarioCancelamento}");

            var connection = _context.Database.GetDbConnection() as SqlConnection;
            if (connection == null)
            {
                System.Diagnostics.Debug.WriteLine("---> ERRO: Conexão SQL NULA.");
                return StatusCode(500, "Erro de infraestrutura: Conexão SQL não encontrada.");
            }

            int rowsAffected = -1; // Para verificar o resultado da Procedure

            try
            {
                await connection.OpenAsync();
                System.Diagnostics.Debug.WriteLine("---> Conexão SQL Aberta.");

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "[CANCEL_AGENDA_UPDATE]";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@idAgenda", request.IdAgenda));
                    command.Parameters.Add(new SqlParameter("@idUsuarioCancelamento", request.IdUsuarioCancelamento));

                    // --- LOG 2: ANTES DE EXECUTAR ---
                    System.Diagnostics.Debug.WriteLine($"---> Executando Procedure [CANCEL_AGENDA_UPDATE] com @idAgenda = {request.IdAgenda}");

                    rowsAffected = await command.ExecuteNonQueryAsync(); // Captura o resultado

                    // --- LOG 3: DEPOIS DE EXECUTAR ---
                    System.Diagnostics.Debug.WriteLine($"---> Procedure executada. Linhas afetadas (retorno ExecuteNonQuery): {rowsAffected}");
                    // Se rowsAffected for 0 ou -1, a Procedure pode não ter encontrado/atualizado o registro.
                }

                return Ok(new { message = $"Agendamento {request.IdAgenda} cancelado com sucesso. (Rows Affected: {rowsAffected})" }); // Adiciona info ao OK
            }
            catch (Exception ex)
            {
                // --- LOG 4: EM CASO DE ERRO SQL ---
                System.Diagnostics.Debug.WriteLine($"*** ERRO CRÍTICO AO EXECUTAR PROCEDURE DE CANCELAMENTO ***");
                System.Diagnostics.Debug.WriteLine($"IdAgenda: {request.IdAgenda}");
                System.Diagnostics.Debug.WriteLine($"Erro: {ex.ToString()}");
                return StatusCode(500, new { message = "Falha ao processar o cancelamento no sistema legado.", detail = ex.Message });
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                    System.Diagnostics.Debug.WriteLine("---> Conexão SQL Fechada.");
                }
            }
        }

        // -----------------------------------------------------------
        // ENDPOINT 7: DE HISTÓRICO
        // -----------------------------------------------------------
        [HttpGet("historico/{idPaciente}")]
        public async Task<IActionResult> GetHistoricoAgendamentos(int idPaciente)
        {
            if (idPaciente <= 0)
            {
                return BadRequest("ID de paciente inválido.");
            }

            try
            {
                // Usando LINQ para montar o histórico (idealmente seria uma VIEW otimizada)
                var historico = await (
                    from agenda in _context.Agendas
                    join atendimento in _context.Tab_Atendimento on agenda.idAtendimento equals atendimento.idAtendimento
                    join medico in _context.PrestadoresMedicos on agenda.IdPrestadorMedico equals medico.IdPrestadorMedico
                    join procedimento in _context.ProcedimentosTussNomenclatura on agenda.IdProcedimentoTussNomenclatura equals procedimento.idProcedimentoTussNomenclatura
                    where agenda.IdPaciente == idPaciente
                    select new AgendamentoHistoricoDto // Usando o DTO do Backend
                    {
                        IdAgenda = agenda.IdAgenda,
                        IdAtendimento = atendimento.idAtendimento,
                        DataHoraMarcada = agenda.DataHoraMarcada ?? DateTime.MinValue,
                        NomePrestador = medico.Apelido ?? medico.Nome ?? "N/D", // Fallback para Nome se Apelido for nulo
                        Especialidade = "Consulta/Exame", // Simplificado (precisaria de JOIN com Cad_ANSCbos)
                        Procedimento = procedimento.PTNDescricaoTuss ?? "N/D",
                        Desativado = agenda.Desativado
                    }
                )
                .OrderByDescending(h => h.DataHoraMarcada)
                .Take(50) // Limita o histórico
                .ToListAsync();

                // O DTO AgendamentoHistoricoDto já tem a lógica do StatusDisplay, se necessário.

                return Ok(historico); // Retorna a lista do DTO
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO AO BUSCAR HISTÓRICO: {ex.ToString()}");
                return StatusCode(500, new { message = "Falha ao buscar histórico de agendamentos." });
            }
        }

        // -----------------------------------------------------------
        // --- MÉTODO AUXILIAR PRIVADO ---
        // -----------------------------------------------------------
        // (INCLUÍDO AQUI - NECESSÁRIO PARA A VERIFICAÇÃO DO DIA ATUAL)
        // -----------------------------------------------------------

        private async Task<bool> TemHorarioFuturoDisponivel(
            int idMedico, int idProcedimento, DateTime data, int duracaoMinutos,
            string? horaInicioStr, string? horaFimStr, string? almocoInicioStr, string? almocoFimStr, bool temAlmoco,
            TimeSpan agora)
        {
            // Validação e Parse dos horários
            if (!TimeSpan.TryParse(horaInicioStr, out TimeSpan inicio) ||
                !TimeSpan.TryParse(horaFimStr, out TimeSpan fim) ||
                duracaoMinutos <= 0 || inicio >= fim)
            {
                return false;
            }
            TimeSpan almocoInicio = TimeSpan.Zero, almocoFim = TimeSpan.Zero;
            if (temAlmoco && (!TimeSpan.TryParse(almocoInicioStr, out almocoInicio) || !TimeSpan.TryParse(almocoFimStr, out almocoFim) || almocoInicio >= almocoFim))
            {
                temAlmoco = false;
            }

            // Busca horários ocupados para o procedimento específico
            var ocupados = await _context.Agendas
                .Where(a => a.IdPrestadorMedico == idMedico &&
                            a.IdProcedimentoTussNomenclatura == idProcedimento && // Usa o IdProcedimento correto
                            a.DataHoraMarcada.HasValue &&
                            a.DataHoraMarcada.Value.Date == data.Date &&
                            !a.Desativado)
                .Select(a => new { Inicio = a.DataHoraMarcada.Value.TimeOfDay, Duracao = a.Minutos ?? duracaoMinutos })
                .ToListAsync();

            // Gera slots e verifica se algum está disponível E é futuro
            TimeSpan horarioAtual = inicio;
            TimeSpan slotDuration = TimeSpan.FromMinutes(duracaoMinutos);

            while (horarioAtual.Add(slotDuration) <= fim)
            {
                TimeSpan slotFim = horarioAtual.Add(slotDuration);
                bool isOcupado = false;

                // Pula almoço
                if (temAlmoco && horarioAtual < almocoFim && slotFim > almocoInicio)
                {
                    horarioAtual = almocoFim;
                    continue;
                }

                // Verifica ocupados
                foreach (var ocupado in ocupados)
                {
                    TimeSpan ocupadoFim = ocupado.Inicio.Add(TimeSpan.FromMinutes(ocupado.Duracao));
                    if (ocupado.Inicio < slotFim && ocupadoFim > horarioAtual)
                    {
                        isOcupado = true;
                        break;
                    }
                }

                // Se NÃO está ocupado E o horário de início é DEPOIS de agora
                if (!isOcupado && horarioAtual > agora)
                {
                    return true; // Encontrou pelo menos um horário futuro disponível
                }

                horarioAtual = horarioAtual.Add(slotDuration);
            }

            return false; // Não encontrou nenhum horário futuro disponível
        }
    }
}