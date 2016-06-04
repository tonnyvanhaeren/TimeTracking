using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
using TimeTracking.IdSrv.configuration;



namespace TimeTracking.IdSrv.Services.Interfaces
{
    public class AuthMessageSender : IMailSender, ISmsSender
    {
        public MailConfig _mailConfig { get; private set; }

        public AuthMessageSender(IOptions<MailConfig> mailConfig)
        {
            _mailConfig = mailConfig.Value;
        }

        public Task SendEmailAsync(string email, string subject, string msg)
        {
            // Plug in your email service here to send an email.
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Tonny Web Site", _mailConfig.UserName));
            message.To.Add(new MailboxAddress("User", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = msg;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {

                client.Connect("smtp.gmail.com", 587); //Gmail

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_mailConfig.UserName, _mailConfig.Password);

                client.Send(message);
                client.Disconnect(true);
            }

            return Task.FromResult(0);
        }

        public Task SendSmsAsync(string number, string message)
        {
            throw new NotImplementedException();
        }
    }
}
