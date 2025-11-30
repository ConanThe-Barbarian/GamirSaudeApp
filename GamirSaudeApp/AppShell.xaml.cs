using GamirSaudeApp.Views;

namespace GamirSaudeApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        this.InitializeComponent();

        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(AgendarConsultaPage), typeof(AgendarConsultaPage));
        Routing.RegisterRoute(nameof(MedicoCalendarioPage), typeof(MedicoCalendarioPage));
        Routing.RegisterRoute(nameof(AgendamentoSucessoPage), typeof(AgendamentoSucessoPage));
        Routing.RegisterRoute(nameof(AgendarExamePage), typeof(AgendarExamePage));
        Routing.RegisterRoute(nameof(ExamesEspecificosPage), typeof(ExamesEspecificosPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(VerifyAccountPage), typeof(VerifyAccountPage));
        Routing.RegisterRoute(nameof(HistoricoPage), typeof(HistoricoPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(ExameCalendarioPage), typeof(ExameCalendarioPage));
        Routing.RegisterRoute(nameof(EsqueciSenhaPage), typeof(EsqueciSenhaPage));
        Routing.RegisterRoute(nameof(RedefinirSenhaPage), typeof(RedefinirSenhaPage));
        Routing.RegisterRoute(nameof(EditarPerfilPage), typeof(EditarPerfilPage));
        Routing.RegisterRoute(nameof(MedicosListaPage), typeof(MedicosListaPage));
        Routing.RegisterRoute(nameof(MedicosExameListaPage), typeof(MedicosExameListaPage));
        Routing.RegisterRoute(nameof(MeusLaudosPage), typeof(MeusLaudosPage));
        Routing.RegisterRoute(nameof(VerLaudoPage), typeof(VerLaudoPage));
    }
}
 