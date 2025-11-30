using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Services
{
    public class UserDataService
    {
        public int IdUserApp { get; set; }
        public string NomeUsuario { get; set; }
        public string EmailUsuario { get; set; }

        // --- NOVAS PROPRIEDADES ---
        public string CpfUsuario { get; set; }
        public string TelefoneUsuario { get; set; }

        public bool ContaVerificada { get; set; }
        public int? IdPacienteGamir { get; set; }

        public string FotoPerfil { get; set; }
        // Propriedade de conveniência para o sistema legado
        public int IdPaciente => IdPacienteGamir ?? 0;

        public void LimparDados()
        {
            IdUserApp = 0;
            NomeUsuario = string.Empty;
            EmailUsuario = string.Empty;
            CpfUsuario = string.Empty;
            TelefoneUsuario = string.Empty;
            ContaVerificada = false;
            IdPacienteGamir = null;
        }
    }
}