using LunchWeb.Helpers;
using LunchWeb.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LunchWeb.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt;

        public EmailSender(IOptions<SmtpOptions> options)
        {
            _opt = options.Value;
        }
        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Cílový E-Mail je povinný!", nameof(to));

            using var message = new MailMessage
            {
                From = new MailAddress(_opt.FromEmail, _opt.FromName),
                Subject = subject ?? string.Empty,
                Body = htmlBody ?? string.Empty,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(new MailAddress(to.Trim()));

            using var client = new SmtpClient
            {
                Host = _opt.Host,
                Port = _opt.Port,
                EnableSsl = _opt.EnableSsl,
                DeliveryFormat = (SmtpDeliveryFormat)SmtpDeliveryMethod.Network,
                Timeout = 10000,
                UseDefaultCredentials = _opt.UseDefaultCredentials
            };

            if (!_opt.UseDefaultCredentials)
                client.Credentials = new NetworkCredential(_opt.User, _opt.Password);

            await client.SendMailAsync(message);
        }
    }
}
