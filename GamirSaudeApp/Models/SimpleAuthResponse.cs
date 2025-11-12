using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    // Modelo genérico para capturar respostas {"message": "..."} da API
    public class SimpleAuthResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}