using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace GamirSaudeApp.Models
{
    public class TipoExame
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("idProcedimento")]
        public int IdProcedimento { get; set; }
    }
}
