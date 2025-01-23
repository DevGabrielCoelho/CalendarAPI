using System.Net;
using System.Text.RegularExpressions;
using CalendarAPI.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace CalendarAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpServer"] ?? throw new ArgumentNullException("Email:SmtpServer");
            _smtpPort = int.TryParse(configuration["Email:SmtpPort"], out var port) ? port : throw new ArgumentException("Invalid SMTP port.");
            _smtpUser = configuration["Email:SmtpUser"] ?? throw new ArgumentNullException("Email:SmtpUser");
            _smtpPass = configuration["Email:SmtpPass"] ?? throw new ArgumentNullException("Email:SmtpPass");
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_smtpUser));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUser, _smtpPass);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while sending the email.", ex);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
            return regex.IsMatch(email);
        }

        public bool IsDomainValid(string email)
        {
            var domain = email.Split('@')[1];
            try
            {
                var hostEntry = Dns.GetHostEntry(domain);
                return hostEntry != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}