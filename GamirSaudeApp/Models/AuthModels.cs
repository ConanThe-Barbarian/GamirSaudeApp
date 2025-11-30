using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Models
{
    // ==================================================
    // DTOs DE REQUISIÇÃO (O que o App envia)
    // ==================================================

    public class LoginRequest
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }

    public class RegisterRequest
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; } // Celular
        public DateTime? DataNascimento { get; set; }
        public string Sexo { get; set; } // "M" ou "F"
        public string Senha { get; set; }
    }

    // Verificação de Conta (SMS)
    public class RequestVerificationRequest
    {
        public string Cpf { get; set; }
        public string Celular { get; set; }
    }

    public class ConfirmVerificationRequest
    {
        public string Cpf { get; set; }
        public string Codigo { get; set; }
    }

    // Recuperação de Senha
    public class EsqueciSenhaRequest
    {
        public string Email { get; set; }
    }

    public class RedefinirSenhaRequest
    {
        public string Email { get; set; }
        public string Codigo { get; set; }
        public string NovaSenha { get; set; }
    }

    // ==================================================
    // DTOs DE RESPOSTA (O que a API devolve)
    // ==================================================

    public class LoginAppResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Token { get; set; } // JWT
        public bool ContaVerificada { get; set; }
        public string FotoPerfil { get; set; } // Base64 String
        public string Message { get; set; } // Mensagem de erro ou sucesso opcional
    }

    // Resposta genérica para operações que só retornam sucesso/falha
    public class SimpleAuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class VerifyAccountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UsuarioInfo Usuario { get; set; } // Dados atualizados após verificar
    }

    // Dados básicos do usuário (usado para atualizar o app após verificação)
    public class UsuarioInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool ContaVerificada { get; set; }
    }


    // DTO para enviar a atualização (PUT)
    // Enviamos apenas o que pode ser alterado
    public class UpdateProfileRequest
    {
        public int IdUserApp { get; set; }
        public string Telefone { get; set; }
        public string FotoPerfil { get; set; } // Opcional (se alterar a foto)
    }

    // ==================================================
    // PERFIL DO USUÁRIO
    // ==================================================

    // DTO para receber os dados completos da API (GET)
    public class UserProfile
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; } // Celular
        public string Sexo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string FotoPerfil { get; set; } // Base64
    }

}
