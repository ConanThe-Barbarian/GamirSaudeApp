using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class RegisterRequestDto
    {
        public string Cpf { get; set; } = string.Empty; // Inicializado
        public string Nome { get; set; } = string.Empty; // Inicializado
        public string Email { get; set; } = string.Empty; // Inicializado
        public string Senha { get; set; } = string.Empty; // Inicializado
        public string? Telefone { get; set; } // Modificado para string? (Pode ser opcional?)
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; } = string.Empty; // Inicializado (Assumindo M ou F)    }
    }
}