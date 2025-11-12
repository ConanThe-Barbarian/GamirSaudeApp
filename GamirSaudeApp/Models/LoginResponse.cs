using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    // Este modelo representa a resposta que a API envia no login bem-sucedido
    public class LoginAppResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("usuario")]
        public UsuarioInfo Usuario { get; set; }
    }

    // Este modelo representa o objeto 'usuario' dentro da resposta
    public class UsuarioInfo
    {
        [JsonPropertyName("idUserApp")]
        public int IdUserApp { get; set; }

        [JsonPropertyName("nomeUserApp")]
        public string NomeUserApp { get; set; }

        [JsonPropertyName("emailUserApp")]
        public string EmailUserApp { get; set; }

        [JsonPropertyName("contaVerificada")]
        public bool ContaVerificada { get; set; }

        [JsonPropertyName("idPacienteGamir")]
        public int? IdPacienteGamir { get; set; }
    }
}