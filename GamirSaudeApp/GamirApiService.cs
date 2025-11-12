using GamirSaudeApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GamirSaudeApp.Services
{
    public class GamirApiService
    {
        private readonly HttpClient _httpClient;

        public GamirApiService()
        {
            _httpClient = new HttpClient();
            string baseAddress; // Usar variável local
          //  baseAddress = "h-ttps://paradingly-unindictable-chandler.ngrok-free.dev";

           //--- INÍCIO DA CORREÇÃO: ENDEREÇO PARA EMULADOR ANDROID ---

            #if ANDROID        

                baseAddress = "http://10.0.2.2:5293";

           #elif IOS
           
                     baseAddress = "http://localhost:5293";
           #elif MACCATALYST

           
                    baseAddress = "http://localhost:5293";
           #elif WINDOWS

                    baseAddress = "http://localhost:5293";
           #else
            
                     baseAddress = "http://localhost:5293";
           #endif

            // --- FIM DA CORREÇÃO ---

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

        public async Task<LoginAppResponse> LoginAsync(string cpf, string senha)
        {
            var request = new LoginRequest { Cpf = cpf, Senha = senha };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    // A resposta da API é lida como o novo modelo LoginAppResponse
                    return await response.Content.ReadFromJsonAsync<LoginAppResponse>();
                }
                else
                {
                    // Tenta ler a mensagem de erro da API em caso de falha (Ex: 401 Unauthorized)
                    var errorResponse = await response.Content.ReadFromJsonAsync<LoginAppResponse>();
                    return errorResponse; // Retorna a resposta com a mensagem de erro
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro no login: {ex.Message}");
                // Retorna nulo apenas em caso de falha de comunicação total
                return null;
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cadastro realizado com sucesso!");
                }
                else
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<LoginAppResponse>(); // Reutilizamos para a Message
                    return (false, errorResponse?.Message ?? "Não foi possível completar o cadastro.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro no registro: {ex.Message}");
                return (false, "Ocorreu um erro de comunicação. Tente novamente.");
            }
        }

        public async Task<IEnumerable<Especialidade>> GetEspecialidadesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Especialidade>>("/api/agendamento/especialidades");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar especialidades: {ex.Message}");
                return Enumerable.Empty<Especialidade>();
            }
        }

        public async Task<IEnumerable<Medico>> GetMedicosAsync(string especialidade)
        {
            try
            {
                var url = $"/api/agendamento/medicos/{especialidade}";
                return await _httpClient.GetFromJsonAsync<IEnumerable<Medico>>(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar médicos: {ex.Message}");
                return Enumerable.Empty<Medico>();
            }
        }

        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisAsync(int idMedico, int mes, int ano)
        {
            // Usa o Model de consulta (SEM IdProcedimento)
            var request = new DiasDisponiveisConsultaRequestDto { IdMedico = idMedico, Mes = mes, Ano = ano };
            try
            {
                // Chama o endpoint de consulta /dias-disponiveis
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/dias-disponiveis", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>() ?? Enumerable.Empty<DiaDisponivel>();
                }
                else
                {
                    // Log de erro para diagnóstico
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Erro API GetDiasDisponiveisAsync (Consulta): Status={response.StatusCode}, Content='{errorContent}'");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao buscar dias (Consulta): {ex.Message}");
            }
            return Enumerable.Empty<DiaDisponivel>();
        }
        public async Task<IEnumerable<string>> GetHorariosDisponiveisAsync(HorariosDisponiveisRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/horarios-disponiveis", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar horários: {ex.Message}");
            }
            return Enumerable.Empty<string>();
        }

        public async Task<bool> AgendarConsultaAsync(AgendamentoRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento", request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // --- MUDANÇA AQUI: Ler e logar o erro do servidor ---
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"ERRO DA API (STATUS: {response.StatusCode}): {errorContent}");
                    // Se for um erro 500, lançamos uma exceção clara
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        throw new Exception($"Erro interno da API. Detalhes: {errorContent}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao agendar consulta: {ex.Message}");
                return false;
            }
        }

        // -----------------------------------------------------------
        // NOVOS MÉTODOS DE SERVIÇO (FLUXO DE EXAME)
        // -----------------------------------------------------------

        // MÉTODO 1 (v5.3.1): Obter Tipos de Exame (JÁ EXISTE, AGORA CORRETO)
        public async Task<IEnumerable<TipoExame>> GetTiposExameAsync()
        {
            try
            {
                // Aponta para o novo endpoint
                return await _httpClient.GetFromJsonAsync<IEnumerable<TipoExame>>("/api/agendamento/exames/tipos");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar tipos de exame: {ex.Message}");
                return Enumerable.Empty<TipoExame>();
            }
        }

        // MÉTODO 2 (v5.3.1): Obter Exames Específicos (JÁ EXISTE, AGORA CORRETO)
        public async Task<IEnumerable<Especialidade>> GetExamesEspecificosAsync(string tipoExame)
        {
            try
            {
                // Usa POST para enviar o nome do tipo
                var request = new { TipoExame = tipoExame };
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/exames/especificos", request);

                if (response.IsSuccessStatusCode)
                {
                    // Retorna como "Especialidade" para reutilizar o modelo
                    return await response.Content.ReadFromJsonAsync<IEnumerable<Especialidade>>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar exames específicos: {ex.Message}");
            }
            return Enumerable.Empty<Especialidade>();
        }

        // NOVO MÉTODO 3: Obter Médicos por PROCEDIMENTO
        public async Task<IEnumerable<Medico>> GetMedicosPorProcedimentoAsync(int idProcedimento)
        {
            try
            {
                var url = $"/api/agendamento/exames/medicos/{idProcedimento}";
                return await _httpClient.GetFromJsonAsync<IEnumerable<Medico>>(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar médicos por procedimento: {ex.Message}");
                return Enumerable.Empty<Medico>();
            }
        }

        // NOVO MÉTODO 4: Obter Dias Disponíveis de EXAME
        public async Task<IEnumerable<DiaDisponivel>> GetDiasDisponiveisExameAsync(DiasDisponiveisRequest request)
        {
            try
            {
                // Chama o novo endpoint de dias de exame
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/exames/dias-disponiveis", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DiaDisponivel>>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar dias de exame: {ex.Message}");
            }
            return Enumerable.Empty<DiaDisponivel>();
        }

        // NOVO MÉTODO 5: Obter Horários Disponíveis de EXAME
        public async Task<IEnumerable<string>> GetHorariosDisponiveisExameAsync(HorariosDisponiveisRequest request)
        {
            try
            {
                // Chama o novo endpoint de horários de exame
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/exames/horarios-disponiveis", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar horários de exame: {ex.Message}");
            }
            return Enumerable.Empty<string>();
        }
    
        public async Task<(bool Success, string Message)> SendVerificationCodeAsync(string email)
        {
            try
            {
                var request = new { Email = email };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/send-verification-code", request);
                var responseContent = await response.Content.ReadFromJsonAsync<LoginAppResponse>(); // Reutilizamos para a Message

                if (response.IsSuccessStatusCode)
                {
                    // Para testes, podemos extrair a mensagem de sucesso junto com o código
                    // Em produção, a mensagem seria apenas "Código enviado..."
                    var message = $"{responseContent?.Message} Código de teste: {responseContent?.Usuario?.ToString() ?? "N/A"}";
                    return (true, responseContent?.Message ?? "Código enviado com sucesso.");
                }
                return (false, responseContent?.Message ?? "Não foi possível enviar o código.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao enviar código: {ex.Message}");
                return (false, "Erro de comunicação.");
            }
        }

        // MÉTODO VerifyAccountAsync ATUALIZADO
        public async Task<(bool Success, string Message, UsuarioInfo UpdatedUser)> VerifyAccountAsync(string email, string codigo)
        {
            try
            {
                var request = new { Email = email, Codigo = codigo };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/verify-account", request);

                // Desserializa a resposta completa
                var responseContent = await response.Content.ReadFromJsonAsync<VerifyAccountResponse>();

                if (response.IsSuccessStatusCode && responseContent?.Usuario != null)
                {
                    // Retorna sucesso, mensagem E os dados do usuário atualizado
                    return (true, responseContent.Message ?? "Conta verificada!", responseContent.Usuario);
                }
                // Retorna falha, mensagem de erro e null para o usuário
                return (false, responseContent?.Message ?? "Não foi possível verificar a conta.", null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao verificar conta: {ex.Message}");
                return (false, "Erro de comunicação.", null);
            }
        }
        public async Task<IEnumerable<AgendamentoHistorico>> GetHistoricoAgendamentosAsync(int idPaciente)
        {
            try
            {
                var url = $"/api/agendamento/historico/{idPaciente}";
                return await _httpClient.GetFromJsonAsync<IEnumerable<AgendamentoHistorico>>(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar histórico: {ex.Message}");
                return Enumerable.Empty<AgendamentoHistorico>();
            }
        }

        // --- NOVO MÉTODO PARA CANCELAR AGENDAMENTO ---
        public async Task<(bool Success, string Message)> CancelarAgendamentoAsync(int idAgenda, int idUserApp)
        {
            try
            {
                var request = new { IdAgenda = idAgenda, IdUsuarioCancelamento = idUserApp };
                var response = await _httpClient.PostAsJsonAsync("/api/agendamento/cancelar", request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Agendamento cancelado com sucesso.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"ERRO DA API NO CANCELAMENTO: {errorContent}");
                    return (false, "Falha ao cancelar agendamento.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro de comunicação no cancelamento: {ex.Message}");
                return (false, "Ocorreu um erro de comunicação.");
            }
        }

        // --- NOVO MÉTODO PARA BUSCAR PERFIL DO USUÁRIO ---
        public async Task<UserProfile> GetUserProfileAsync(int idUserApp)
        {
            if (idUserApp <= 0) return null; // Validação básica

            try
            {
                var url = $"/api/auth/profile/{idUserApp}";
                // A API retorna UserProfileDto, mas desserializamos diretamente para o nosso modelo UserProfile
                var profile = await _httpClient.GetFromJsonAsync<UserProfile>(url);
                System.Diagnostics.Debug.WriteLine($"Perfil carregado para IdUserApp {idUserApp}: {profile?.NomeUsuario}"); // Log
                return profile;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar perfil do usuário: {ex.Message}");
                return null; // Retorna nulo em caso de falha
            }
        }

        // -----------------------------------------------------------
        // MÉTODOS DE RECUPERAÇÃO DE SENHA (ATUALIZADOS)
        // -----------------------------------------------------------

        /// <summary>
        /// Chama a API para solicitar um código de redefinição de senha.
        /// </summary>
        public async Task<(bool Success, string Message)> EsqueciSenhaAsync(string email)
        {
            try
            {
                var request = new EsqueciSenhaRequest { Email = email };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/esqueci-senha", request);

                // Lê o conteúdo da resposta, seja sucesso (200) ou erro (404)
                var responseContent = await response.Content.ReadFromJsonAsync<SimpleAuthResponse>();

                // --- ALTERAÇÃO AQUI ---
                if (response.IsSuccessStatusCode)
                {
                    // Sucesso (200 OK)
                    return (true, responseContent?.Message ?? "Solicitação processada.");
                }
                else
                {
                    // Erro (404 Not Found ou outro)
                    return (false, responseContent?.Message ?? "E-mail não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao solicitar código de redefinição: {ex.Message}");
                return (false, "Não foi possível conectar ao servidor.");
            }
        }

        /// <summary>
        /// Chama a API para redefinir a senha usando o código.
        /// </summary>
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
                    return (false, responseContent?.Message ?? "Código inválido or expirado.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao redefinir senha: {ex.Message}");
                return (false, "Não foi possível conectar ao servidor.");
            }
        }
    }
}

    
