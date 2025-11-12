using GamirSaudeApp.Views;

namespace GamirSaudeApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        this.InitializeComponent();

        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(AgendarConsultaPage), typeof(AgendarConsultaPage));
        Routing.RegisterRoute(nameof(MedicosDisponiveisPage), typeof(MedicosDisponiveisPage));
        Routing.RegisterRoute(nameof(AgendamentoSucessoPage), typeof(AgendamentoSucessoPage));
        Routing.RegisterRoute(nameof(AgendarExamePage), typeof(AgendarExamePage));
        Routing.RegisterRoute(nameof(ExamesEspecificosPage), typeof(ExamesEspecificosPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(VerifyAccountPage), typeof(VerifyAccountPage));
        Routing.RegisterRoute(nameof(HistoricoPage), typeof(HistoricoPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(MedicosExamePage), typeof(MedicosExamePage));
        Routing.RegisterRoute(nameof(EsqueciSenhaPage), typeof(EsqueciSenhaPage));
        Routing.RegisterRoute(nameof(RedefinirSenhaPage), typeof(RedefinirSenhaPage));
    }
}
 