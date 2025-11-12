using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Domain.Entities
{
    [Table("Cad_Paciente")]
    public class Paciente
    {

        // Chave primária da tabela
        [Key]
        public int IdPaciente { get; set; }

        // Nome do paciente
        [Column("Paciente")]
        public string? Nome { get; set; }

        // CPF do paciente, usado para login
        public string? Cpf { get; set; }

        // E-mail do paciente
        public string? Email { get; set; }

        // Senha criptografada do paciente para acesso ao app
        public string? SenhaWeb { get; set; }

        // Quantidade de pontos no programa de fidelidade
        public int? QtdePontosFidelidade { get; set; }

        // Flag para indicar se o cadastro está desativado
        public bool Desativado { get; set; }
    }
}
