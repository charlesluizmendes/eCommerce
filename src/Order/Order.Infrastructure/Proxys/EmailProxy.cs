using Order.Infrastructure.Options;
using Order.Domain.Interfaces.Proxys;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Order.Infrastructure.Proxys
{
    public class EmailProxy : IEmailProxy
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailProxy(IOptions<EmailConfiguration> emailOptions)
        {
            _smtpHost = emailOptions.Value.SmtpHost;
            _smtpPort = emailOptions.Value.SmtpPort;
            _smtpUser = emailOptions.Value.SmtpUser;
            _smtpPass = emailOptions.Value.SmtpPass;
        }

        public async Task SendAsync(string from, string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Text) { Text = html };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
