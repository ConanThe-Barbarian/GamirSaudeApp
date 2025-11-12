using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    public class Medico
    {
        // Garante o mapeamento correto do JSON "idPrestadorMedico"
        [JsonPropertyName("idPrestadorMedico")]
        public int Id { get; set; }

        // Garante o mapeamento correto do JSON "nome"
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("Apelido")]
        public string Apelido { get; set; }
    }
}