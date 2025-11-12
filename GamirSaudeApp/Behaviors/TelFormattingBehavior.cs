using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using System.Linq;

namespace GamirSaudeApp.Behaviors
{
    public class TelFormattingBehavior : Behavior<Entry>
    {
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

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.NewTextValue)) return;

            var entry = sender as Entry;
            var digits = new string(args.NewTextValue.Where(char.IsDigit).ToArray());

            if (digits.Length > 11)
            {
                digits = digits.Substring(0, 11);
            }

            string formattedText = digits;
            if (digits.Length > 10) // Celular com 9º dígito
            {
                formattedText = $"({digits.Substring(0, 2)}) {digits.Substring(2, 5)}-{digits.Substring(7)}";
            }
            else if (digits.Length > 6) // Celular ou Fixo
            {
                formattedText = $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)}-{digits.Substring(6)}";
            }
            else if (digits.Length > 2)
            {
                formattedText = $"({digits.Substring(0, 2)}) {digits.Substring(2)}";
            }

            if (entry.Text != formattedText)
                entry.Text = formattedText;
        }
    }
}
