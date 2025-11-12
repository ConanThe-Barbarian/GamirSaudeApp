using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    // Modelo para a resposta do endpoint /verify-account
    public class VerifyAccountResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("usuario")]
        public UsuarioInfo Usuario { get; set; } // Reutiliza UsuarioInfo de LoginAppResponse
    }
}