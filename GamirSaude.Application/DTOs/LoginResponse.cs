using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Token { get; set; } // O JWT para ele acessar rotas privadas
        public bool ContaVerificada { get; set; }
        public string? FotoPerfil { get; set; } // Base64
    }
}