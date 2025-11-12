using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
// GamirSaude.Application/DTOs/CancelamentoRequestDto.cs



namespace GamirSaude.Application.DTOs
{
    public class CancelamentoRequestDto
    {
        [Required]
        public int IdAgenda { get; set; }

        [Required]
        public int IdUsuarioCancelamento { get; set; }
    }
}