using GamirSaudeApp.Models;
using System.Net.Http.Json;
using System.Diagnostics;

namespace GamirSaudeApp.Services
{
    public class GamirApiService
    {
        private readonly HttpClient _httpClient;

        public GamirApiService()
        {
            _httpClient = new HttpClient();

            // Ajuste o IP conforme necessário (localhost ou 10.0.2.2)
            string baseAddress = "http://localhost:5293";

            if (!string.IsNullOrEmpty(baseAddress))
            {
                try
                {
                    _httpClient.BaseAddress = new Uri(baseAddress);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[GamirApiService] ERRO URI: {ex.Message}");
                }
            }
        }

        // ==================================================================
        // 1. AUTENTICAÇÃO E PERFIL
        // ==================================================================

        public async Task<LoginResponse> LoginAsync(string cpf, string senha)
        {
            try
            {
                var request = new LoginRequest { Cpf = cpf, Senha = senha };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<LoginResponse>();

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro no Login: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
                if (response.IsSuccessStatusCode) return (true, "Sucesso");
                return (false, await response.Content.ReadAsStringAsync() ?? "Erro");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<bool> SolicitarVerificacaoAsync(string cpf, string celular)
        {
            try
            {
                var dados = new RequestVerificationRequest { Cpf = cpf, Celular = celular };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/request-verification", dados);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<bool> ConfirmarVerificacaoAsync(string cpf, string codigo)
        {
            try
            {
                var dados = new ConfirmVerificationRequest { Cpf = cpf, Codigo = codigo };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/confirm-verification", dados);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ==================================================================
        // 2. FLUXO CLÍNICO (AGENDAMENTO)
        // ==================================================================

        public async Task<IEnumerable<Especialidade>> GetEspecialidadesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Especialidade>>("/api/agendamento/especialidades");
            }
            catch { return Enumerable.Empty<Especialidade>(); }
        }

        public async Task<IEnumerable<Medico>> GetMedicosAsync(string especialidade)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Medico>>($"/api/agendamento/medicos/{especialidade}");
            }
            catch { return Enumerable.Empty<Medico>(); }
        }

        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisAsync(int idMedico, int mes, int ano)
        {
            try
            {
                var request = new DiasDisponiveisRequest { IdMedico = idMedico, Mes = mes, Ano = ano, IdProcedimento = null };
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/dias-disponiveis", request);
                if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>();
            }
            catch { }
            return Enumerable.Empty<DiaDisponivel>();
        }

        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisExameAsync(DiasDisponiveisRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/dias-disponiveis", request);
                if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>();
            }
            catch { }
            return Enumerable.Empty<DiaDisponivel>();
        }

        public async Task<IEnumerable<string>> GetHorariosDisponiveisAsync(HorariosDisponiveisRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/horarios-disponiveis", request);
                if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            }
            catch { }
            return Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<string>> GetHorariosDisponiveisExameAsync(HorariosDisponiveisRequest request) => await GetHorariosDisponiveisAsync(request);

        public async Task<bool> AgendarConsultaAsync(AgendamentoRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento", request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // ==================================================================
        // 3. HISTÓRICO E LAUDOS (O MÉTODO QUE FALTAVA)
        // ==================================================================

        public async Task<IEnumerable<AgendamentoHistorico>> GetHistoricoAgendamentosAsync(int idPaciente)
        {
            try
            {
                // Este é o método que o compilador estava reclamando
                return await _httpClient.GetFromJsonAsync<IEnumerable<AgendamentoHistorico>>($"/api/agendamento/historico/{idPaciente}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro histórico: {ex.Message}");
                return Enumerable.Empty<AgendamentoHistorico>();
            }
        }

        public async Task<IEnumerable<LaudoModel>> GetMeusLaudosAsync(int idPaciente)
        {
            try
            {
                // Rota deve bater com [Route("api/[controller]")] -> api/laudos
                return await _httpClient.GetFromJsonAsync<IEnumerable<LaudoModel>>($"/api/laudos/{idPaciente}");
            }
            catch { return Enumerable.Empty<LaudoModel>(); }
        }

        // ==================================================================
        // 4. OUTROS (Recuperação de Senha, Perfil)
        // ==================================================================

        public async Task<(bool Success, string Message)> EsqueciSenhaAsync(string email)
        {
            try
            {
                var request = new EsqueciSenhaRequest { Email = email };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/esqueci-senha", request);
                return (response.IsSuccessStatusCode, "Código enviado");
            }
            catch { return (false, "Erro"); }
        }

        public async Task<(bool Success, string Message)> RedefinirSenhaAsync(string email, string codigo, string novaSenha)
        {
            try
            {
                var request = new RedefinirSenhaRequest { Email = email, Codigo = codigo, NovaSenha = novaSenha };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/redefinir-senha", request);
                return (response.IsSuccessStatusCode, "Senha redefinida");
            }
            catch { return (false, "Erro"); }
        }

        public async Task<UserProfile> GetUserProfileAsync(int idUserApp)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserProfile>($"/api/auth/profile/{idUserApp}");
            }
            catch { return null; }
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("/api/auth/profile", request);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
    }
}