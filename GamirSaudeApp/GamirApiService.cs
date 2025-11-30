using GamirSaudeApp.Models;
using System.Net.Http.Json;
using System.Diagnostics;

namespace GamirSaudeApp.Services
{
    public class GamirApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public GamirApiService()
        {
            _httpClient = new HttpClient();

            string baseAddress; // Usar variável local

            baseAddress = "http://localhost:5293";

            // Define o BaseAddress somente se a string foi definida corretamente
            if (!string.IsNullOrEmpty(baseAddress))
            {
                try
                {
                    _httpClient.BaseAddress = new Uri(baseAddress);
                    Debug.WriteLine($"[GamirApiService] Configurado para API em: {baseAddress}");
                }
                catch (UriFormatException ex)
                {
                    Debug.WriteLine($"[GamirApiService] ERRO: Formato de URI inválido para BaseAddress: {baseAddress}. Erro: {ex.Message}");
                    _httpClient.BaseAddress = new Uri("http://127.0.0.1:1"); // Força falha controlada
                }
            }
            else
            {
                Debug.WriteLine("[GamirApiService] ERRO CRÍTICO: BaseAddress não pôde ser determinado pelas diretivas de compilação.");
                _httpClient.BaseAddress = new Uri("http://127.0.0.1:1"); // Força falha controlada
            }
        }



        // ==================================================================
        // 1. AUTENTICAÇÃO E PERFIL
        // ==================================================================

        // Altere de LoginAppResponse para LoginResponse
        public async Task<LoginResponse> LoginAsync(string cpf, string senha)
        {
            try
            {
                var request = new LoginRequest { Cpf = cpf, Senha = senha };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    // Agora usa o nome correto da classe unificada
                    return await response.Content.ReadFromJsonAsync<LoginResponse>();
                }
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
                if (response.IsSuccessStatusCode)
                    return (true, "Cadastro realizado com sucesso!");

                var error = await response.Content.ReadAsStringAsync();
                return (false, error ?? "Erro no cadastro.");
            }
            catch (Exception ex)
            {
                return (false, $"Erro de comunicação: {ex.Message}");
            }
        }

        // Verificação de Conta (SMS)
        public async Task<bool> SolicitarVerificacaoAsync(string cpf, string celular)
        {
            try
            {
                var dados = new { Cpf = cpf, Celular = celular };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/request-verification", dados);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro solicitar verificação: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ConfirmarVerificacaoAsync(string cpf, string codigo)
        {
            try
            {
                var dados = new { Cpf = cpf, Codigo = codigo };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/confirm-verification", dados);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro confirmar verificação: {ex.Message}");
                return false;
            }
        }

        // ==================================================================
        // 2. FLUXO DE CONSULTAS
        // ==================================================================

        public async Task<IEnumerable<Especialidade>> GetEspecialidadesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Especialidade>>("/api/agendamento/especialidades");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro especialidades: {ex.Message}");
                return Enumerable.Empty<Especialidade>();
            }
        }

        public async Task<IEnumerable<Medico>> GetMedicosAsync(string especialidade)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Medico>>($"/api/agendamento/medicos/{especialidade}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro médicos: {ex.Message}");
                return Enumerable.Empty<Medico>();
            }
        }

        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisAsync(int idMedico, int mes, int ano)
        {
            try
            {
                var request = new { IdMedico = idMedico, Mes = mes, Ano = ano };
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/dias-disponiveis", request);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro dias consulta: {ex.Message}");
            }
            return Enumerable.Empty<DiaDisponivel>();
        }

        public async Task<IEnumerable<string>> GetHorariosDisponiveisAsync(HorariosDisponiveisRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/horarios-disponiveis", request);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro horários: {ex.Message}");
            }
            return Enumerable.Empty<string>();
        }

        // ==================================================================
        // 3. FLUXO DE EXAMES
        // ==================================================================

        public async Task<IEnumerable<TipoExame>> GetTiposExameAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<TipoExame>>("/api/agendamento/exames/tipos");
            }
            catch { return Enumerable.Empty<TipoExame>(); }
        }

        public async Task<IEnumerable<Especialidade>> GetExamesEspecificosAsync(string tipoExame)
        {
            try
            {
                var request = new { TipoExame = tipoExame };
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/exames/especificos", request);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<Especialidade>>();
            }
            catch { }
            return Enumerable.Empty<Especialidade>();
        }

        public async Task<IEnumerable<Medico>> GetMedicosPorProcedimentoAsync(int idProcedimento)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Medico>>($"/api/agendamento/exames/medicos/{idProcedimento}");
            }
            catch { return Enumerable.Empty<Medico>(); }
        }

        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisExameAsync(DiasDisponiveisRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/exames/dias-disponiveis", request);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>();
            }
            catch { }
            return Enumerable.Empty<DiaDisponivel>();
        }

        public async Task<IEnumerable<string>> GetHorariosDisponiveisExameAsync(HorariosDisponiveisRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/exames/horarios-disponiveis", request);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
            }
            catch { }
            return Enumerable.Empty<string>();
        }

        // ==================================================================
        // 4. AGENDAMENTO FINAL (Compartilhado)
        // ==================================================================

        public async Task<bool> AgendarConsultaAsync(AgendamentoRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento", request);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[API] Erro Agendar: {error}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Exceção Agendar: {ex.Message}");
                return false;
            }
        }

        // ==================================================================
        // 5. HISTÓRICO E OUTROS
        // ==================================================================

        public async Task<IEnumerable<AgendamentoHistorico>> GetHistoricoAgendamentosAsync(int idPaciente)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<AgendamentoHistorico>>($"/api/agendamento/historico/{idPaciente}");
            }
            catch { return Enumerable.Empty<AgendamentoHistorico>(); }
        }

        public async Task<IEnumerable<LaudoModel>> GetMeusLaudosAsync(int idPaciente)
        {
            try
            {
                // Endpoint simulado para laudos
                return await _httpClient.GetFromJsonAsync<IEnumerable<LaudoModel>>($"/api/laudos/{idPaciente}");
            }
            catch { return Enumerable.Empty<LaudoModel>(); }
        }

        // -----------------------------------------------------------
        // MÉTODOS DE RECUPERAÇÃO DE SENHA
        // -----------------------------------------------------------

        public async Task<(bool Success, string Message)> EsqueciSenhaAsync(string email)
        {
            try
            {
                // Nota: Certifique-se que 'EsqueciSenhaRequest' está em AuthModels.cs
                var request = new EsqueciSenhaRequest { Email = email };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/esqueci-senha", request);

                // Tenta ler a resposta padrão
                var responseContent = await response.Content.ReadFromJsonAsync<SimpleAuthResponse>();

                if (response.IsSuccessStatusCode)
                {
                    return (true, responseContent?.Message ?? "Código enviado com sucesso!");
                }
                else
                {
                    return (false, responseContent?.Message ?? "E-mail não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao solicitar código: {ex.Message}");
                return (false, "Erro de comunicação com o servidor.");
            }
        }

        public async Task<(bool Success, string Message)> RedefinirSenhaAsync(string email, string codigo, string novaSenha)
        {
            try
            {
                var request = new RedefinirSenhaRequest
                {
                    Email = email,
                    Codigo = codigo,
                    NovaSenha = novaSenha
                };

                var response = await _httpClient.PostAsJsonAsync("/api/auth/redefinir-senha", request);
                var responseContent = await response.Content.ReadFromJsonAsync<SimpleAuthResponse>();

                if (response.IsSuccessStatusCode)
                {
                    return (true, responseContent?.Message ?? "Senha redefinida com sucesso!");
                }
                else
                {
                    return (false, responseContent?.Message ?? "Código inválido ou erro ao redefinir.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao redefinir senha: {ex.Message}");
                return (false, "Erro de comunicação com o servidor.");
            }
        }

        // --- MÉTODOS DE PERFIL ---

        // Busca os dados (Já existia, mas confirmando)
        public async Task<UserProfile> GetUserProfileAsync(int idUserApp)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserProfile>($"/api/auth/profile/{idUserApp}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar perfil: {ex.Message}");
                return null;
            }
        }

        // Atualiza os dados (NOVO)
        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("/api/auth/profile", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao atualizar perfil: {ex.Message}");
                return false;
            }
        }
    }
}