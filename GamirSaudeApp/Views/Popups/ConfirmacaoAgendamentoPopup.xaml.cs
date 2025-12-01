using CommunityToolkit.Maui.Views;

namespace GamirSaudeApp.Views.Popups
{
    public partial class ConfirmacaoAgendamentoPopup : Popup
    {
        public ConfirmacaoAgendamentoPopup(string detalhes)
        {
            InitializeComponent();
            lblDetalhes.Text = detalhes;
        }

        private void OnConfirmarClicked(object sender, EventArgs e)
        {
            // Fecha retornando TRUE
            Close(true);
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            // Fecha retornando FALSE
            Close(false);
        }
    }
}