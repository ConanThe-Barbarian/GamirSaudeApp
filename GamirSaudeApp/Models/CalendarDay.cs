using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GamirSaudeApp.Models
{
    public partial class CalendarDay : ObservableObject // Ou apenas 'public class CalendarDay'
    {
        public DateTime Date { get; set; }
        public string Status { get; set; } // "Disponivel", "PoucasVagas", "Indisponivel"
        public bool IsEmpty { get; set; } // Para dias vazios no início/fim do mês

        [ObservableProperty] // Para a UI atualizar a seleção visualmente
        private bool isSelected;

        // Propriedade calculada para o número do dia
        public string DayNumber => IsEmpty ? "" : Date.Day.ToString();
    }
}