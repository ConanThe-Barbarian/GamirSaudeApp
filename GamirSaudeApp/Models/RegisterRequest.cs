using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Models
{
    public class RegisterRequest
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string? Telefone { get; set; }
        public DateTime? DataNascimento { get; set; } // <-- ADICIONAR
        public string? Sexo { get; set; }

    }
}