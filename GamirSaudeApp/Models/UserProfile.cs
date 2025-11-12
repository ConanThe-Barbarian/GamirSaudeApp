using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization; // Necessário para mapear nomes diferentes

namespace GamirSaudeApp.Models
{
    public class UserProfile
    {
        // Correção: Mapeia "idUserApp" para a propriedade IdUserApp
        [JsonPropertyName("idUserApp")]
        public int IdUserApp { get; set; }

        // Correção: Mapeia "cpfUsuario" (do DTO) para CpfUsuario
        [JsonPropertyName("cpfUsuario")]
        public string CpfUsuario { get; set; }

        // Correção: Mapeia "nomeUsuario" (do DTO) para NomeUsuario
        [JsonPropertyName("nomeUsuario")]
        public string NomeUsuario { get; set; }

        // Correção: Mapeia "telefoneUsuario" (do DTO) para TelefoneUsuario
        [JsonPropertyName("telefoneUsuario")]
        public string? TelefoneUsuario { get; set; }

        // Correção: Mapeia "emailUsuario" (do DTO) para EmailUsuario
        [JsonPropertyName("emailUsuario")]
        public string EmailUsuario { get; set; }

        // Correção: Mapeia "dataNascimentoUsuario" (do DTO) para DataNascimentoUsuario
        [JsonPropertyName("dataNascimentoUsuario")]
        public DateTime? DataNascimentoUsuario { get; set; }

        // Correção: Mapeia "sexoUsuario" (do DTO) para SexoUsuario
        [JsonPropertyName("sexoUsuario")]
        public string? SexoUsuario { get; set; }

        [JsonPropertyName("contaVerificada")]
        public bool ContaVerificada { get; set; }

        [JsonPropertyName("idPacienteGamir")]
        public int? IdPacienteGamir { get; set; }

        // Propriedades calculadas (elas já usam os nomes corretos das propriedades acima)
        public string SexoDisplay => SexoUsuario == "M" ? "Masculino" : (SexoUsuario == "F" ? "Feminino" : "Não informado");
        public string DataNascimentoDisplay => DataNascimentoUsuario?.ToString("dd/MM/yyyy") ?? "Não informada";
    }
}