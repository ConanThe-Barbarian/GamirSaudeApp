using GamirSaudeApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace GamirSaudeApp.Views
{
    public partial class AgendamentoSucessoPage : ContentPage
    {
        public AgendamentoSucessoPage(SucessoAgendamentoViewModel viewModel) // <-- Injete o ViewModel
        {
            InitializeComponent();
            this.BindingContext = viewModel; // Atribua o contexto
        }
    }
}