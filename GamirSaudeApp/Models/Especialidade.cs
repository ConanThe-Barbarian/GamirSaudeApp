using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    public class Especialidade
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        // Mapeia o campo "IdProcedimento" que o Controller está a retornar no JSON
        [JsonPropertyName("idProcedimento")]
        public int IdProcedimento { get; set; }
    }
}