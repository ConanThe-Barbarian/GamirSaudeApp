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

            // Endereço da API (Ajuste conforme necessário: localhost ou IP)
            string baseAddress = "http://localhost:5293";
            // Para Android Emulator use: "http://10.0.2.2:5293"

            if (!string.IsNullOrEmpty(baseAddress))
            {
                try
                {
                    _httpClient.BaseAddress = new Uri(baseAddress);
                    Debug.WriteLine($"[GamirApiService] Configurado para API em: {baseAddress}");
                }
                catch (UriFormatException ex)
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
                {
                    // Agora usa o DTO unificado LoginResponse
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

        public async Task<bool> SolicitarVerificacaoAsync(string cpf, string celular)
        {
            try
            {
                var dados = new RequestVerificationRequest { Cpf = cpf, Celular = celular };
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
                var dados = new ConfirmVerificationRequest { Cpf = cpf, Codigo = codigo };
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
        // 2. FLUXO CLÍNICO UNIFICADO (CONSULTAS E EXAMES)
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

        // --- BUSCA DE DIAS (Unificada) ---
        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisAsync(int idMedico, int mes, int ano)
        {
            try
            {
                var request = new DiasDisponiveisRequest
                {
                    IdMedico = idMedico,
                    Mes = mes,
                    Ano = ano,
                    IdProcedimento = null // Consulta médica padrão
                };

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

        // Método específico de exame (Redireciona para a mesma rota)
        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisExameAsync(DiasDisponiveisRequest request)
        {
            try
            {
                // A mágica acontece aqui: mesma rota, mas o objeto 'request' tem IdProcedimento preenchido
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/dias-disponiveis", request);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Erro dias exame: {ex.Message}");
            }
            return Enumerable.Empty<DiaDisponivel>();
        }

        // --- BUSCA DE HORÁRIOS (Unificada) ---
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

        public async Task<IEnumerable<string>> GetHorariosDisponiveisExameAsync(HorariosDisponiveisRequest request)
        {
            // Reutiliza a lógica acima
            return await GetHorariosDisponiveisAsync(request);
        }

        // ==================================================================
        // 3. AGENDAMENTO FINAL (POST)
        // ==================================================================

        public async Task<bool> AgendarConsultaAsync(AgendamentoRequest request)
        {
            try
            {
                // Endpoint único para agendar (o Controller decide se é exame ou consulta)
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
        // 4. RECUPERAÇÃO DE SENHA E PERFIL
        // ==================================================================

        public async Task<(bool Success, string Message)> EsqueciSenhaAsync(string email)
        {
            try
            {
                var request = new EsqueciSenhaRequest { Email = email };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/esqueci-senha", request);
                var responseContent = await response.Content.ReadFromJsonAsync<SimpleAuthResponse>();

                if (response.IsSuccessStatusCode)
                    return (true, responseContent?.Message ?? "Código enviado!");

                return (false, responseContent?.Message ?? "E-mail não encontrado.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao solicitar código: {ex.Message}");
                return (false, "Erro de comunicação.");
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
                    return (true, responseContent?.Message ?? "Senha redefinida!");

                return (false, responseContent?.Message ?? "Falha na redefinição.");
            }
            catch (Exception ex)
            {
                return (false, $"Erro: {ex.Message}");
            }
        }

        public async Task<UserProfile> GetUserProfileAsync(int idUserApp)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserProfile>($"/api/auth/profile/{idUserApp}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("/api/auth/profile", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}