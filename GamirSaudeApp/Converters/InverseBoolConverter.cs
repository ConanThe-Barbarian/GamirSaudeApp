using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace GamirSaudeApp.Converters
{
    // Nossa classe implementa a interface IValueConverter
    public class InverseBoolConverter : IValueConverter
    {
        // Este é o método que faz a tradução "de ida" (do ViewModel para a View)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Pega o valor booleano e retorna a sua negação (!)
            return !(bool)value;
        }

        // Este é o método que faz a tradução "de volta" (da View para o ViewModel), que não usaremos agora
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}