using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
// Arquivo: Models/HorarioModel.cs
// Motivo: Criar um objeto para o binding, forçando o Android a renderizar o texto.

namespace GamirSaudeApp.Models
{
    // Agora herda de ObservableObject para notificar mudanças na tela
    public partial class HorarioModel : ObservableObject
    {
        public string Hora { get; set; }

        [ObservableProperty]
        private bool isSelected;
    }
}