using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    public class DiaDisponivel
    {
        [JsonPropertyName("dia")]
        public int Dia { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
