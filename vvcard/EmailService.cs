using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace vvcard
{
    public class EmailService : IEmailSender
    {
        private readonly IConfiguration Configuration;
        public EmailService(IConfiguration configuration) {
            Configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("VVCard.ru - письмо сгенерировано автоматически и не требует ответа", "mail@vvcard.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            var hostName = Configuration["EmailSettings:Hosting"];
            var port = int.Parse(Configuration["EmailSettings:Port"]);
            var userName = Configuration["EmailSettings:UserName"];
            var password = Configuration["EmailSettings:Password"];
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(hostName, port);
                await client.AuthenticateAsync(userName, password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
