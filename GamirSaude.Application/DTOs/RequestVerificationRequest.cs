using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GamirSaude.Application.DTOs
{
    public class RequestVerificationRequest
    {
        [Required]       
        public string Cpf { get; set; }

        // Opcional: Celular, caso a API SMGH decida enviar por SMS
        public string? Celular { get; set; }
    }
}