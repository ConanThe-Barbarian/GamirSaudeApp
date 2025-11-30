using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GamirSaude.Application.DTOs
{
    // ============================================================
    // AUTENTICAÇÃO (Login e Registro)
    // ============================================================

    public class LoginRequest
    {
        public string Cpf { get; set; }
        public string Senha { get; set; }
    }

    public class LoginResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool ContaVerificada { get; set; }
        public string FotoPerfil { get; set; } // Base64
        public string Token { get; set; }
    }

    public class RegisterRequest
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Sexo { get; set; } // 'M' ou 'F'
        public string Senha { get; set; }
    }

    // ============================================================
    // VERIFICAÇÃO DE CONTA (SMS/OTP)
    // ============================================================

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

    // ============================================================
    // RECUPERAÇÃO DE SENHA (Novo Fluxo)
    // ============================================================

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

    // DTO genérico para respostas simples (Sucesso/Erro)
    public class SimpleAuthResponse
    {
        public string Message { get; set; }
    }

    // ============================================================
    // PERFIL DO USUÁRIO
    // ============================================================

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Sexo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string FotoPerfil { get; set; }
        public bool ContaVerificada { get; set; }
    }

    public class UpdateProfileRequest
    {
        public int IdUserApp { get; set; }
        public string Telefone { get; set; }
        public string FotoPerfil { get; set; } // Base64
    }
}