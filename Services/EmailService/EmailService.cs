﻿using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace EmailSenderApp.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config
)
        {
            _config = config;

        }
        public void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.ContactName;
            email.Body = new TextPart(TextFormat.Html) { Text = "<h1>" + request.Body + "</h1>" };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailHost").Value, int.Parse(_config.GetSection("Port").Value), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }



    }
}
