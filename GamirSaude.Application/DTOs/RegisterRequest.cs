using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GamirSaude.Application.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cpf { get; set; } // Vamos limpar a máscara no Controller

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Senha { get; set; }

        public string Telefone { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Sexo { get; set; }
    }
}