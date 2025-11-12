using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Services
{
    public class UserDataService
    {
        // Propriedades do usuário do APP
        public int IdUserApp { get; set; }
        public string NomeUsuario { get; set; }
        public string EmailUsuario { get; set; }
        public bool ContaVerificada { get; set; }

        // ID do paciente no sistema legado (preenchido após verificação)
        public int? IdPacienteGamir { get; set; }

        // Mantemos esta propriedade para o fluxo de agendamento que já funciona
        public int IdPaciente => IdPacienteGamir ?? 0;

        // --- MÉTODO CLEAR DATA (GARANTIR QUE ESTEJA COMPLETO) ---
        public void ClearData()
        {
            IdUserApp = 0;
            NomeUsuario = string.Empty;
            EmailUsuario = string.Empty;
            ContaVerificada = false;
            IdPacienteGamir = null; // Garante que IdPaciente também volte a ser 0
            System.Diagnostics.Debug.WriteLine("--- UserDataService.ClearData() Executado ---"); // Log de confirmação
        }
        // --- FIM DO MÉTODO CLEAR DATA ---
    }
}