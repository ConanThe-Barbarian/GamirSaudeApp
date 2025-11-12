using BCrypt.Net;
using GamirSaude.Application.DTOs;
using GamirSaude.Domain.Entities;
using GamirSaude.Domain.Interfaces;
using GamirSaude.Infrastructure.Persistence;
using GamirSaude.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
namespace GamirSaude.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly GamirSaudeDbContext _context;
        private readonly IEmailService _emailService;
        public AuthController(IPacienteRepository pacienteRepository, GamirSaudeDbContext context, IEmailService emailService)
        {
            _pacienteRepository = pacienteRepository;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            // ALTERAÇÃO: Agora buscamos na tabela de usuários do App
            var usuarioApp = await _context.UsuariosApp
        .FirstOrDefaultAsync(u => u.CpfUserApp == loginRequest.Cpf);
            // ALTERAÇÃO: Verificamos o hash da senha
            if (usuarioApp == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Senha, usuarioApp.SenhaUserApp))
            {
                return Unauthorized(new { message = "CPF ou senha inválidos." });
            }

            // Se o login for bem-sucedido...
            return Ok(new
            {
                message = "Login bem-sucedido!",
                usuario = new // Retornamos dados do usuário do app
                {
                    usuarioApp.IdUserApp,
                    usuarioApp.NomeUserApp,
                    usuarioApp.EmailUserApp,
                    usuarioApp.ContaVerificada,
                    usuarioApp.idPacienteGamir
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (await _context.UsuariosApp.AnyAsync(u => u.CpfUserApp == request.Cpf))
            {
                return Conflict(new { message = "Este CPF já está em uso." });
            }

            if (await _context.UsuariosApp.AnyAsync(u => u.EmailUserApp == request.Email))
            {
                return Conflict(new { message = "Este e-mail já está em uso." });
            }

            // ALTERAÇÃO: Criamos o hash da senha
            var senhaHasheada = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var novoUsuario = new Cad_UsuarioApp
            {
                CpfUserApp = new string(request.Cpf?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>()),
                NomeUserApp = request.Nome,
                EmailUserApp = request.Email,
                SenhaUserApp = senhaHasheada, // <-- Usamos o hash
                TelUserApp = request.Telefone,
                DataNascUserApp = request.DataNascimento, // <-- CAMPO NOVO
                SexoUserApp = !string.IsNullOrEmpty(request.Sexo) ? request.Sexo.Substring(0, 1) : null,
                ContaVerificada = false,
                idPacienteGamir = null
            };

            _context.UsuariosApp.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário cadastrado com sucesso!" });
        }
        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendCodeRequestDto request)
        {
            // 1. Encontrar o usuário pelo e-mail
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.EmailUserApp == request.Email);
            if (usuario == null)
            {
                // Resposta ambígua por segurança: não revela se o e-mail existe
                return Ok(new { message = "Se o e-mail estiver cadastrado, o código será enviado." });
            }

            // 2. Verificar se a conta já está verificada
            if (usuario.ContaVerificada)
            {
                return BadRequest(new { message = "Esta conta já foi verificada." });
            }

            // 3. Gerar o código e a data de expiração
            var codigo = new Random().Next(100000, 999999).ToString("D6");
            var dataExpiracao = DateTime.UtcNow.AddMinutes(15);

            // 4. Salvar no banco de dados
            usuario.CodigoVerificacao = codigo;
            usuario.DataExpiracaoCodigo = dataExpiracao;
            await _context.SaveChangesAsync();

            // 5. CHAMAR O SERVIÇO DE E-MAIL REAL
            var emailSent = await _emailService.SendVerificationCodeAsync(
                usuario.EmailUserApp,
                usuario.NomeUserApp,
                codigo
            );

            if (!emailSent)
            {
                // Log de falha de e-mail aqui (mas ainda retorna OK para o usuário por segurança)
            }

            // 6. REMOVER O CÓDIGO DE TESTE DA RESPOSTA. Retorna uma mensagem de sucesso simples.
            return Ok(new
            {
                message = "O código de verificação foi enviado para o seu e-mail.",
                // REMOVIDO: verificationCodeForTesting = codigo 
            });
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerifyCodeRequestDto request)
        {
            // 1. Encontrar o usuário do app pelo e-mail
            var usuarioApp = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.EmailUserApp == request.Email);

            if (usuarioApp == null)
            {
                return BadRequest(new { message = "Usuário não encontrado." });
            }

            // 2. Realizar as validações de segurança (Assumindo que você corrigiu a lógica de validação)
            if (usuarioApp.ContaVerificada || usuarioApp.CodigoVerificacao != request.Codigo || usuarioApp.DataExpiracaoCodigo < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Falha na verificação. O código pode ser inválido, expirado ou a conta já está verificada." });
            }

            // 3. O passo crucial: Encontrar o paciente no sistema legado
            var pacienteLegado = await _context.Pacientes.FirstOrDefaultAsync(p => p.Email == request.Email);

            if (pacienteLegado == null)
            {
                return NotFound(new { message = "Não foi possível encontrar um registro de paciente correspondente." });
            }

            // --- PASSO CRÍTICO: GARANTIR O REGISTRO DO PLANO PARTICULAR (ID 33) ---
            const int ID_PLANO_CONVENIO_PARTICULAR = 33;

            // Verifica se o paciente já tem o plano particular (33)
            bool planoExiste = await _context.Cad_PacientePlano.AnyAsync(pp =>
                pp.idPaciente == pacienteLegado.IdPaciente &&
                pp.idPlanoConvenio == ID_PLANO_CONVENIO_PARTICULAR);

            if (!planoExiste)
            {
                // Se não existir, insere o registro mínimo na Cad_PacientePlano
                var novoPlanoPadrao = new Cad_PacientePlano
                {
                    idPaciente = pacienteLegado.IdPaciente,
                    idPlanoConvenio = ID_PLANO_CONVENIO_PARTICULAR,
                    // Colunas obrigatórias com valores padrão para o App
                    DataAtivacaoPlano = DateTime.Now,
                    Titular = true,
                    Desativado = false
                };

                _context.Cad_PacientePlano.Add(novoPlanoPadrao);
            }
            // --- FIM DO PASSO CRÍTICO ---


            // 4. Se todas as validações passaram, atualizamos o usuário do app
            usuarioApp.ContaVerificada = true;
            usuarioApp.idPacienteGamir = pacienteLegado.IdPaciente; // A ponte foi criada!

            // 5. Limpar os dados de verificação por segurança
            usuarioApp.CodigoVerificacao = null;
            usuarioApp.DataExpiracaoCodigo = null;

            // Salva todas as alterações (Cad_PacientePlano e Cad_UsuarioApp)
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Conta verificada com sucesso!",
                usuario = new // Retorna os dados atualizados
                {
                    usuarioApp.IdUserApp,
                    usuarioApp.NomeUserApp,
                    usuarioApp.EmailUserApp,
                    usuarioApp.ContaVerificada, // Será true
                    usuarioApp.idPacienteGamir // O ID crucial
                }
            });
        }

        // -----------------------------------------------------------
        // ENDPOINT NOVO: BUSCAR PERFIL DO USUÁRIO
        // -----------------------------------------------------------
        [HttpGet("profile/{idUserApp}")]
        public async Task<IActionResult> GetUserProfile(int idUserApp)
        {
            if (idUserApp <= 0)
            {
                return BadRequest("ID de usuário inválido.");
            }

            try
            {
                // Busca o usuário na tabela Cad_UsuarioApp pelo ID
                var usuario = await _context.UsuariosApp
                                        .AsNoTracking() // Otimização para consulta de leitura
                                        .FirstOrDefaultAsync(u => u.IdUserApp == idUserApp);

                if (usuario == null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }

                // Mapeia a entidade para o DTO de resposta
                var profileDto = new UserProfileDto
                {
                    IdUserApp = usuario.IdUserApp,
                    CpfUsuario = usuario.CpfUserApp, // Envia sem máscara
                    NomeUsuario = usuario.NomeUserApp,
                    TelefoneUsuario = usuario.TelUserApp, // Envia sem máscara
                    EmailUsuario = usuario.EmailUserApp,
                    DataNascimentoUsuario = usuario.DataNascUserApp,
                    SexoUsuario = usuario.SexoUserApp, // 'M' ou 'F'
                    ContaVerificada = usuario.ContaVerificada,
                    IdPacienteGamir = usuario.idPacienteGamir
                };

                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO AO BUSCAR PERFIL: {ex.ToString()}");
                return StatusCode(500, new { message = "Falha ao buscar dados do perfil." });
            }
        }

// -----------------------------------------------------------
        // ENDPOINTS DE RECUPERAÇÃO DE SENHA (ATUALIZADOS)
        // -----------------------------------------------------------

        /// <summary>
        /// Endpoint 1: Usuário informa o e-mail que esqueceu a senha.
        /// </summary>
        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequestDto request)
        {
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.EmailUserApp == request.Email);

            // --- ALTERAÇÃO AQUI ---
            // Validação explícita solicitada pelo usuário.
            if (usuario == null)
            {
                Debug.WriteLine($"Solicitação de redefinição para e-mail inexistente: {request.Email}");
                // Retorna 404 Not Found com a mensagem de erro específica.
                return NotFound(new { message = "Por favor verificar o e-mail digitado, não encontramos esse e-mail em nosso sistema" });
            }

            try
            {
                // Gerar código e expiração
                var codigo = new Random().Next(100000, 999999).ToString("D6");
                var dataExpiracao = DateTime.UtcNow.AddMinutes(15); // 15 minutos de validade

                // Salvar no banco
                usuario.CodigoVerificacao = codigo;
                usuario.DataExpiracaoCodigo = dataExpiracao;
                await _context.SaveChangesAsync();

                // Enviar e-mail
                var emailSent = await _emailService.SendPasswordResetCodeAsync(
                    usuario.EmailUserApp,
                    usuario.NomeUserApp,
                    codigo
                );

                if (!emailSent)
                {
                    Debug.WriteLine($"Falha no serviço de e-mail ao tentar enviar código de redefinição para {request.Email}.");
                    // Não informa o usuário da falha do e-mail, apenas loga.
                }

                // Se o e-mail existiu e o código foi gerado, retorna 200 OK.
                return Ok(new { message = "Um código de redefinição foi enviado para o seu e-mail." });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro inesperado em [esqueci-senha]: {ex.Message}");
                return StatusCode(500, new { message = "Ocorreu um erro interno no servidor." });
            }
        }

        /// <summary>
        /// Endpoint 2: Usuário envia o código e a nova senha.
        /// </summary>
        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequestDto request)
        {
            // 1. Encontrar o usuário
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.EmailUserApp == request.Email);

            if (usuario == null)
            {
                // Não encontramos o usuário
                return BadRequest(new { message = "O código de redefinição é inválido ou expirou." });
            }

            // 2. Validar o código e a expiração
            if (usuario.CodigoVerificacao != request.Codigo || usuario.DataExpiracaoCodigo < DateTime.UtcNow)
            {
                // Código errado ou expirado
                return BadRequest(new { message = "O código de redefinição é inválido ou expirou." });
            }

            // 3. Tudo válido! Atualizar a senha
            var novaSenhaHasheada = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            usuario.SenhaUserApp = novaSenhaHasheada;

            // 4. Limpar os dados de verificação
            usuario.CodigoVerificacao = null;
            usuario.DataExpiracaoCodigo = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Senha redefinida com sucesso!" });
        }
    }
}    
 