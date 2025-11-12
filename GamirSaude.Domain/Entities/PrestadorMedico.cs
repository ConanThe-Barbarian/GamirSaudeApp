using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_PrestadorMedico")]
    public class PrestadorMedico
    {
        // Chave primária da tabela de prestadores
        [Key]
        public int IdPrestadorMedico { get; set; }

        [Column("PrestadorMedico")] // <-- A CORREÇÃO ESTÁ AQUI
        public string? Nome { get; set; }

        [Column("Apelido")]
        public string Apelido { get; set; }

        // CRM ou outro registro profissional
        public string? Crm { get; set; }

        // Flag que indica se este profissional trabalha com agendamentos 
        public bool AtendeComAgenda { get; set; }

        // Flag para indicar se o cadastro está desativado
        public bool Desativado { get; set; }
    }   

}


