// GamirSaude.Infrastructure/Services/EmailService.cs

using GamirSaude.Domain.Interfaces;
using Microsoft.Extensions.Configuration; // Para ler o appsettings.json
using System.Diagnostics;
using System.Net;
using System.Net.Mail; // Para SmtpClient e MailMessage
using System.Threading.Tasks;
using System;

namespace GamirSaude.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromEmail = "no-reply@gamirsaude.com.br";
        private readonly string _fromName = "Gamir Saúde";

        // Construtor que recebe a configuração injetada
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Carrega as credenciais de forma segura do appsettings.json
            _smtpHost = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";

            // Usa TryParse e fallback seguro
            if (!int.TryParse(_configuration["EmailSettings:Port"], out _smtpPort))
            {
                _smtpPort = 587; // Porta padrão segura
            }

            _smtpUser = _configuration["EmailSettings:User"] ?? string.Empty;
            _smtpPassword = _configuration["EmailSettings:Password"] ?? string.Empty;
        }

        public async Task<bool> SendVerificationCodeAsync(string toEmail, string userName, string code)
        {
            // Verifica se as credenciais críticas foram carregadas
            if (string.IsNullOrEmpty(_smtpUser) || string.IsNullOrEmpty(_smtpPassword))
            {
                Debug.WriteLine("ERRO CRÍTICO: Credenciais SMTP ausentes no appsettings.json.");
                return false;
            }

            try
            {
                var fromAddress = new MailAddress(_fromEmail, _fromName);
                var toAddress = new MailAddress(toEmail, userName);

                var subject = "Seu Código de Verificação Gamir Saúde";

                // --- NOVO CORPO DO E-MAIL COM HTML INCORPORADO (Estilo V2) ---
                var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
                    .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); overflow: hidden; }}
                    .header {{ 
                        background: linear-gradient(to right, #51bd54, #0098da); /* Usando o verde do app e azul */
                        min-height: 200px;
                        text-align: center;
                        background-image: url('https://conanthe-barbarian.github.io/G-C-Innovations/images/logo_gamir_saude.png');
                        background-size: 65%;
                        background-position: center center;
                        background-repeat: no-repeat;
                        /* Fixes para Outlook */
                        mso-line-height-rule: exactly; 
                        line-height: 0;
                        font-size: 0;
                        padding: 0;
                    }}
                    .content {{ padding: 0px; color: #333333; text-align: center; }}
                    .details {{ 
                        background-color: #f9f9f9; 
                        padding: 15px; 
                        border-radius: 5px; 
                        color: #333333; 
                        text-align: center; 
                        margin: 20px auto;
                        width: fit-content; 
                        min-width: 150px; 
                        max-width: 80%; 
                        display: block; 
                    }}
                    .details h2 {{ 
                        color: #0098da; 
                        margin: 0; 
                        font-size: 28px; 
                        font-weight: bold;
                        border: none;
                    }}
                    .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #777777; border-top: 1px solid #eeeeee; }}
                    .footer a {{ color: #0098da; text-decoration: none; }}
                </style>
            </head>
            <body bgcolor='#f4f4f4'>
                <table width='100%' bgcolor='#f4f4f4' border='0' cellpadding='0' cellspacing='0'>
                    <tr>
                        <td align='center'>
                            <div class='container'>
                                <div class='header'>
                                    </div>
                                <div class='content'>
                                    <p>Olá, <b>{userName}</b>.</p>
                                    <p>Seu código de verificação para acesso ao aplicativo Gamir Saúde é:</p>
                                    <div class='details'>
                                        <h2>{code}</h2>
                                    </div>
                                    <p>Por favor, utilize este código na tela de verificação do aplicativo. Ele expira em 15 minutos.</p>
                                    <p>Atenciosamente,<br>Equipe Gamir Saúde</p>
                                </div>
                                <div class='footer'>
                                    <p>Mensagem enviada automaticamente pela Equipe Gamir Saúde</p>
                                    <p>&copy; © 2024 Gamir Saúde | AMIC - ASSISTÊNCIA MÉDICA INTEGRADA LTDA </p>
                                    <p>CNPJ: 19.900.956/0001-78</>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </body>           
       </html>";   var smtp = new SmtpClient
                {
                    Host = _smtpHost,
                    Port = _smtpPort,
                    // ESTAS CONFIGURAÇÕES SÃO OBRIGATÓRIAS PARA O GMAIL:
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    // Usa o endereço de envio (o que está no campo User do appsettings.json)
                    Credentials = new NetworkCredential(_smtpUser, _smtpPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    // TENTA ENVIAR O E-MAIL
                    await smtp.SendMailAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                // SE FALHAR, REGISTRA NO LOG E RETORNA FALSE
                Debug.WriteLine($"ERRO FATAL NO ENVIO DE E-MAIL (GMAIL/SMTP): {ex.Message}");
                // Retornar false garante que o usuário no app receba uma mensagem de falha
                return false;
            }
        }

        // --- MÉTODO ATUALIZADO ---
        public async Task<bool> SendPasswordResetCodeAsync(string toEmail, string userName, string code)
        {
            if (string.IsNullOrEmpty(_smtpUser) || string.IsNullOrEmpty(_smtpPassword))
            {
                Debug.WriteLine("ERRO CRÍTICO: Credenciais SMTP ausentes no appsettings.json.");
                return false;
            }

            try
            {
                var fromAddress = new MailAddress(_fromEmail, _fromName);
                var toAddress = new MailAddress(toEmail, userName);

                var subject = "Recuperação de Senha - Gamir Saúde";

                // --- SEU HTML (IDÊNTICO AO DE VERIFICAÇÃO) ---
                var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); overflow: hidden; }}
                        .header {{ 
                            background: linear-gradient(to right, #51bd54, #0098da); /* Usando o verde do app e azul */
                            min-height: 200px;
                            text-align: center;
                            background-image: url('https://conanthe-barbarian.github.io/G-C-Innovations/images/logo_gamir_saude.png');
                            background-size: 65%;
                            background-position: center center;
                            background-repeat: no-repeat;
                            /* Fixes para Outlook */
                            mso-line-height-rule: exactly; 
                            line-height: 0;
                            font-size: 0;
                            padding: 0;
                        }}
                        .content {{ padding: 0px; color: #333333; text-align: center; }}
                        .details {{ 
                            background-color: #f9f9f9; 
                            padding: 15px; 
                            border-radius: 5px; 
                            color: #333333; 
                            text-align: center; 
                            margin: 20px auto;
                            width: fit-content; 
                            min-width: 150px; 
                            max-width: 80%; 
                            display: block; 
                        }}
                        .details h2 {{ 
                            color: #0098da; 
                            margin: 0; 
                            font-size: 28px; 
                            font-weight: bold;
                            border: none;
                        }}
                        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #777777; border-top: 1px solid #eeeeee; }}
                        .footer a {{ color: #0098da; text-decoration: none; }}
                    </style>
                </head>
                <body bgcolor='#f4f4f4'>
                    <table width='100%' bgcolor='#f4f4f4' border='0' cellpadding='0' cellspacing='0'>
                        <tr>
                            <td align='center'>
                                <div class='container'>
                                    <div class='header'>
                                        </div>
                                    <div class='content'>
                                        <p>Olá, <b>{userName}</b>.</p>
                                        
                                        <p>Recebemos uma solicitação para redefinir sua senha. Use o código abaixo:</p>
                                        <div class='details'>
                                            <h2>{code}</h2>
                                        </div>
                                        <p>Utilize este código na tela de redefinição de senha. Ele expira em 15 minutos.</p>
                                        <p>Se você não solicitou isso, pode ignorar este e-mail com segurança.</p>
                                        <p>Atenciosamente,<br>Equipe Gamir Saúde</p>
                                        </div>
                                    <div class='footer'>
                                        <p>Mensagem enviada automaticamente pela Equipe Gamir Saúde</p>
                                        <p>&copy; © 2024 Gamir Saúde | AMIC - ASSISTÊNCIA MÉDICA INTEGRADA LTDA </p>
                                        <p>CNPJ: 19.900.956/0001-78</>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </body>           
           </html>";

                var smtp = new SmtpClient
                {
                    Host = _smtpHost,
                    Port = _smtpPort,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpUser, _smtpPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    await smtp.SendMailAsync(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO FATAL NO ENVIO DE E-MAIL (REDEFINIÇÃO): {ex.Message}");
                return false;
            }
        }
    }
}