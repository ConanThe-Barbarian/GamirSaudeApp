using CommunityToolkit.Maui.Views;

namespace GamirSaudeApp.Views.Popups
{
    public partial class LogoutPopup : Popup
    {
        public LogoutPopup()
        {
            InitializeComponent();

            // DEFININDO A PROPRIEDADE VIA CÓDIGO
            // Isso contorna o erro do XAML e mantém a funcionalidade.
            
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(false);
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            Close(true);
        }
    }
}