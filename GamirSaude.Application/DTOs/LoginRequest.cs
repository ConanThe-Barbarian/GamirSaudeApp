using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GamirSaude.Application.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string Cpf { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}