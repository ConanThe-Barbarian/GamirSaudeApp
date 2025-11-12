using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GamirSaude.Domain.Entities
{
    [Table("Cad_Agenda")]
    public class Agenda
    {
        [Key]
        public int IdAgenda { get; set; }
        public int? IdPaciente { get; set; }
        public DateTime? DataHoraMarcada { get; set; }
        [Column("idProcedimentoTussNomenclatura")]
        public int? IdProcedimentoTussNomenclatura { get; set; }
        public int? IdPrestadorMedico { get; set; }
        public int? Minutos { get; set; }
        public bool Confirmado { get; set; }
        public DateTime? DataHoraCancelamento { get; set; }
        public bool Desativado { get; set; } // Assumindo que esta coluna existe

        // --- ADICIONADO PARA JOIN E PROCEDURE ---
        public int? idAtendimento { get; set; }
        public int? Numero { get; set; } // Coluna de Número de Encaixe
        public int? idUsuarioAuto { get; set; }
        public string? Paciente { get; set; } // Nome do Paciente (redundante, mas usado na Procedure)
        public DateTime? DataHoraInclusao { get; set; }
        public int? idPlanoConvenio { get; set; } // Usado na Procedure
        public int? idPrestadorMedicoEspecialidade { get; set; } // Usado na Procedure

    }
}