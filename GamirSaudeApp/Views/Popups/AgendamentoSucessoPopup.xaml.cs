using CommunityToolkit.Maui.Views;

namespace GamirSaudeApp.Views.Popups
{
    public partial class AgendamentoSucessoPopup : Popup
    {
        public AgendamentoSucessoPopup()
        {
            InitializeComponent();
        }

        private void OnOkClicked(object sender, EventArgs e)
        {
            // Fecha o popup devolvendo o controle para a ViewModel
            Close();
        }
    }
}