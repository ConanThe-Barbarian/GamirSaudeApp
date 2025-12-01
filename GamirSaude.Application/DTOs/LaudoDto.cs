using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Application.DTOs
{
    public class LaudoDto
    {
        public int Id { get; set; }
        public string NomeExame { get; set; }      // Ex: "Hemograma Completo"
        public string NomeMedico { get; set; }     // Ex: "Laboratório Central"
        public DateTime DataExame { get; set; }
        public string Status { get; set; }         // "Disponível", "Em Análise"
        public string UrlPdf { get; set; }         // O Link para o arquivo
    }
}