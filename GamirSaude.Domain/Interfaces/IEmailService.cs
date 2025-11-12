using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamirSaude.Domain.Interfaces

    {
        // Abstração do serviço de e-mail para a Clean Architecture
        public interface IEmailService
        {
         Task<bool> SendVerificationCodeAsync(string toEmail, string userName, string code);
         Task<bool> SendPasswordResetCodeAsync(string toEmail, string userName, string code);
    }

    }
