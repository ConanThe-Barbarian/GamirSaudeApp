using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class UpdateProfileRequest
    {
        public int IdUserApp { get; set; }
        public string Telefone { get; set; }
        public string FotoPerfil { get; set; }
    }
}