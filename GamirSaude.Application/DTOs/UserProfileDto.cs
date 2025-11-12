using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// GamirSaude.Application/DTOs/UserProfileDto.cs

using System;

namespace GamirSaude.Application.DTOs
{
    // DTO para exibir os dados do perfil do usuário
    public class UserProfileDto
    {
public int IdUserApp { get; set; }
        public string? NomeUsuario { get; set; } // Modificado para string?
        public string? EmailUsuario { get; set; } // Modificado para string?
        public string? CpfUsuario { get; set; } // Modificado para string?
        public string? TelefoneUsuario { get; set; } // Modificado para string?
        public DateTime? DataNascimentoUsuario { get; set; }
        public string? SexoUsuario { get; set; } // Modificado para string?
        public bool ContaVerificada { get; set; }
        public int? IdPacienteGamir { get; set; }    }
}