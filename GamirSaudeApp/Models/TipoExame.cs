using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Globalization;

namespace GamirSaudeApp.Models
{
    public class TipoExame
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Ex: RX ANTEBRAÇO
        public string PrazoResultado { get; set; } // Ex: 2 dias úteis
        public decimal Valor { get; set; } // Ex: 150.00

        public string ValorFormatado => Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
    }
}