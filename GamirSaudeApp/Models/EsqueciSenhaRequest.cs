using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    // Modelo para enviar a solicitação de Esqueci Senha
    public class EsqueciSenhaRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}