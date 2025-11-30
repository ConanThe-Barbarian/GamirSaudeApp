using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Sexo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string FotoPerfil { get; set; } // Base64
        public bool ContaVerificada { get; set; }
    }
}