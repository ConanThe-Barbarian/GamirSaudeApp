using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class LoginRequestDto
    {
        // Adicionamos a inicialização para garantir que nunca será null
        public string Cpf { get; set; } = string.Empty;

        // Adicionamos a inicialização para garantir que nunca será null
        public string Senha { get; set; } = string.Empty;
    }
}
