using GamirSaude.Application.DTOs;
using GamirSaude.Application.Services; // <--- IMPORTANTE PARA ITokenService
using GamirSaude.Domain.Entities;
using GamirSaude.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GamirSaude.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GamirSaudeDbContext _context;
        private readonly ITokenService _tokenService; // <--- CAMPO QUE FALTAVA (Erro CS0103)

        // Construtor com Injeção de Dependência
        public AuthController(GamirSaudeDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var cpfLimpo = request.Cpf.Replace(".", "").Replace("-", "").Trim();

            if (await _context.UsuariosApp.AnyAsync(u => u.Cpf == cpfLimpo))
                return BadRequest("Este CPF já está cadastrado.");

            if (await _context.UsuariosApp.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Este E-mail já está cadastrado.");

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var novoUsuario = new UsuarioApp
            {
                Nome = request.Nome,
                Cpf = cpfLimpo,
                Email = request.Email,
                Telefone = request.Telefone,
                DataNascimento = request.DataNascimento,
                Sexo = request.Sexo?.Substring(0, 1),
                SenhaHash = senhaHash,
                ContaVerificada = false
            };

            _context.UsuariosApp.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário cadastrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var cpfLimpo = request.Cpf.Replace(".", "").Replace("-", "").Trim();

            var usuario = await _context.UsuariosApp
                .FirstOrDefaultAsync(u => u.Cpf == cpfLimpo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                return Unauthorized("CPF ou Senha inválidos.");
            }

            // Gera o Token usando o serviço injetado
            string token = _tokenService.GenerateToken(usuario);

            // CORREÇÃO DO ERRO CS0128: Garantimos que a variável response é única
            var loginResponse = new LoginResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                ContaVerificada = usuario.ContaVerificada,
                FotoPerfil = usuario.FotoPerfilBase64,
                Token = token
            };

            return Ok(loginResponse);
        }

        // 1. PEDIR VERIFICAÇÃO (CPF + Celular)
        [HttpPost("request-verification")]
        public async Task<IActionResult> RequestVerification([FromBody] RequestVerificationRequest request)
        {
            // Limpa máscara do CPF vindo do App
            var cpfLimpo = request.Cpf.Replace(".", "").Replace("-", "").Trim();

            // A. Busca o usuário pelo CPF
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.Cpf == cpfLimpo);

            // Regra de Segurança: Mesmo se não achar, não devemos dar dicas. 
            // Mas para desenvolvimento, vamos retornar o erro claro.
            if (usuario == null) return NotFound("Usuário não encontrado com este CPF.");

            if (usuario.ContaVerificada) return BadRequest("Esta conta já está verificada.");

            // B. Atualiza o telefone (caso o usuário tenha informado um diferente no cadastro)
            // Isso garante que o SMS vá para o número que ele está com a mão agora.
            usuario.Telefone = request.Celular;

            // C. Gera o Código (Simulação do retorno da API SMGH)
            var codigo = new Random().Next(100000, 999999).ToString();

            // D. Salva no banco
            usuario.CodigoVerificacao = codigo;
            usuario.DataExpiracaoCodigo = DateTime.Now.AddMinutes(10); // SMS geralmente expira mais rápido

            _context.UsuariosApp.Update(usuario);
            await _context.SaveChangesAsync();

            // E. MOCK: Simula envio de SMS (Aqui entraria a chamada da API SMGH)
            Console.WriteLine($"[SMS MOCK] Enviando código {codigo} para {request.Celular}");

            // Retorna o código no corpo SÓ PARA TESTE (Remova em produção!)
            return Ok(new { message = "Código SMS enviado!", debug_codigo = codigo });
        }

        // 2. CONFIRMAR CÓDIGO (CPF + Código)
        [HttpPost("confirm-verification")]
        public async Task<IActionResult> ConfirmVerification([FromBody] ConfirmVerificationRequest request)
        {
            var cpfLimpo = request.Cpf.Replace(".", "").Replace("-", "").Trim();

            // A. Busca o usuário pelo CPF
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.Cpf == cpfLimpo);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            // B. Valida o Código
            if (usuario.CodigoVerificacao != request.Codigo)
                return BadRequest("Código inválido.");

            if (usuario.DataExpiracaoCodigo < DateTime.Now)
                return BadRequest("Código expirado. Solicite um novo.");

            // C. SUCESSO: Simula vínculo com legado e libera
            usuario.ContaVerificada = true;
            usuario.IdPacienteLegado = 88922; // Mock do ID legado

            // Limpeza
            usuario.CodigoVerificacao = null;
            usuario.DataExpiracaoCodigo = null;

            _context.UsuariosApp.Update(usuario);
            await _context.SaveChangesAsync();

            // D. Gera novo token atualizado
            var novoToken = _tokenService.GenerateToken(usuario);

            return Ok(new
            {
                message = "Celular verificado com sucesso!",
                contaVerificada = true,
                token = novoToken
            });
        }

        // ============================================================
        // MÉTODOS DE PERFIL (FALTAVAM ESTES!)
        // ============================================================

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var usuario = await _context.UsuariosApp.FindAsync(id);

            if (usuario == null) return NotFound("Usuário não encontrado.");

            var perfil = new UserProfileDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Cpf = usuario.Cpf, // Já está limpo no banco
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Sexo = usuario.Sexo,
                DataNascimento = usuario.DataNascimento,
                FotoPerfil = usuario.FotoPerfilBase64,
                ContaVerificada = usuario.ContaVerificada
            };

            return Ok(perfil);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var usuario = await _context.UsuariosApp.FindAsync(request.IdUserApp);

            if (usuario == null) return NotFound("Usuário não encontrado.");

            // Atualiza apenas o permitido
            usuario.Telefone = request.Telefone;

            // Se enviou foto nova, atualiza. Se enviou string vazia/null, ignora (ou limpa, dependendo da regra)
            if (!string.IsNullOrEmpty(request.FotoPerfil))
            {
                usuario.FotoPerfilBase64 = request.FotoPerfil;
            }

            _context.UsuariosApp.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Perfil atualizado com sucesso!" });
        }

        // ============================================================
        // 3. RECUPERAÇÃO DE SENHA (NOVO)
        // ============================================================

        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequest request)
        {
            // 1. Validação básica
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new SimpleAuthResponse { Message = "O e-mail é obrigatório." });

            // 2. Busca o usuário no BANCO DO APP
            // Importante: Usamos o banco local, pois é lá que o email e senha hashada vivem.
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
            {
                // Retornamos 404 com mensagem clara para o App exibir
                return NotFound(new SimpleAuthResponse { Message = "E-mail não encontrado." });
            }

            // 3. Gera o Código (Simulação de 6 dígitos)
            var codigo = new Random().Next(100000, 999999).ToString();

            // 4. Salva no banco para validar depois
            usuario.CodigoVerificacao = codigo;
            usuario.DataExpiracaoCodigo = DateTime.Now.AddMinutes(15); // Validade de 15 min

            _context.UsuariosApp.Update(usuario);
            await _context.SaveChangesAsync();

            // 5. LOG (Simula o envio de e-mail no CONSOLE da API)
            // ATENÇÃO: Quando testar, olhe o terminal preto da API para ver este código!
            Console.WriteLine($"\n[EMAIL MOCK] -------------------------------------------------");
            Console.WriteLine($"[EMAIL MOCK] Recuperação de Senha para: {request.Email}");
            Console.WriteLine($"[EMAIL MOCK] CÓDIGO: {codigo}");
            Console.WriteLine($"[EMAIL MOCK] -------------------------------------------------\n");

            return Ok(new SimpleAuthResponse { Message = "Código de verificação enviado para o seu e-mail." });
        }

        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
        {
            // 1. Validações
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Codigo) ||
                string.IsNullOrWhiteSpace(request.NovaSenha))
            {
                return BadRequest(new SimpleAuthResponse { Message = "Preencha todos os campos." });
            }

            // 2. Busca usuário
            var usuario = await _context.UsuariosApp.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (usuario == null) return NotFound(new SimpleAuthResponse { Message = "Usuário não encontrado." });

            // 3. Verifica o Código e a Validade
            if (usuario.CodigoVerificacao != request.Codigo)
                return BadRequest(new SimpleAuthResponse { Message = "Código inválido." });

            if (usuario.DataExpiracaoCodigo < DateTime.Now)
                return BadRequest(new SimpleAuthResponse { Message = "O código expirou. Solicite um novo." });

            // 4. Atualiza a senha (HASHING COM BCRYPT É OBRIGATÓRIO)
            // Isso garante que a senha salva seja criptografada igual ao registro
            string novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            usuario.SenhaHash = novaSenhaHash;

            // 5. Limpa o código usado (Segurança para não reutilizar)
            usuario.CodigoVerificacao = null;
            usuario.DataExpiracaoCodigo = null;

            _context.UsuariosApp.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new SimpleAuthResponse { Message = "Senha redefinida com sucesso! Faça login com a nova senha." });
        }
    }
}