using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaudeApp.Converters // Certifique-se que o namespace corresponde à sua estrutura de pastas
{
    /// <summary>
    /// Converte um valor booleano para uma espessura (Thickness).
    /// Útil para alterar a espessura da borda com base em um estado (ex: IsSelected).
    /// </summary>
    public class BoolToThicknessConverter : IValueConverter
    {
        /// <summary>
        /// O valor da espessura a ser retornado quando o booleano for True. Padrão é 1.
        /// </summary>
        public double TrueValue { get; set; } = 1;

        /// <summary>
        /// O valor da espessura a ser retornado quando o booleano for False. Padrão é 0.
        /// </summary>
        public double FalseValue { get; set; } = 0;

        /// <summary>
        /// Converte o valor booleano para Thickness.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verifica se o valor recebido é um booleano
            if (value is bool isTrue)
            {
                // Se for true, retorna um Thickness com o valor de TrueValue
                // Se for false, retorna um Thickness com o valor de FalseValue
                return new Thickness(isTrue ? TrueValue : FalseValue);
            }

            // Se o valor não for booleano, retorna uma espessura padrão (ou pode lançar exceção)
            return new Thickness(FalseValue);
        }

        /// <summary>
        /// A conversão reversa não é implementada/necessária para este cenário.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported for BoolToThicknessConverter.");
        }
    }
}