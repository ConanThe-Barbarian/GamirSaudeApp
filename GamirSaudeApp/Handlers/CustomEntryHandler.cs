using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS
using Microsoft.Maui.Controls.Platform; // Usaremos este para a compatibilidade
#endif

namespace GamirSaudeApp.Handlers
{
    public static class CustomHandlers
    {
        public static void Apply()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
#if WINDOWS
                // Remove a cor de fundo padrão
                handler.PlatformView.Background = null;

                // Remove a borda e o fundo que aparecem quando o controle está em foco
                handler.PlatformView.FocusVisualPrimaryBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
                handler.PlatformView.FocusVisualSecondaryBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
#endif
            });
        }
    }
}