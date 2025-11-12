using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GamirSaudeApp.Behaviors
{
    public class CpfFormattingBehavior : Behavior<Entry>
    {
        private string _previousText = "";

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Entry entry)
            {
                // Evita loop infinito
                if (entry.Text == _previousText)
                    return;

                // Remove todos os caracteres não numéricos
                string cleanText = new string(args.NewTextValue?.Where(char.IsDigit).ToArray());

                // Limita a 11 caracteres
                if (cleanText.Length > 11)
                {
                    cleanText = cleanText.Substring(0, 11);
                }

                // Aplica a formatação do CPF
                string formattedText = FormatCpf(cleanText);

                _previousText = formattedText;
                entry.Text = formattedText;

                // Move o cursor para o final
                entry.CursorPosition = formattedText.Length;
            }
        }

        private string FormatCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) return cpf;

            // Se tiver 11 dígitos, formata como CPF: 000.000.000-00
            if (cpf.Length == 11)
            {
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
            }
            // Se tiver mais de 3 dígitos mas menos de 6, formata como: 000.000
            else if (cpf.Length > 3 && cpf.Length <= 6)
            {
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3)}";
            }
            // Se tiver mais de 6 dígitos mas menos de 9, formata como: 000.000.000
            else if (cpf.Length > 6 && cpf.Length <= 9)
            {
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6)}";
            }

            return cpf;
        }
    }
}
