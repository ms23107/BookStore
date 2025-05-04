using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BookStore.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromEmail = "MS_KpxnRf@trial-351ndgwj6nxlzqx8.mlsender.net";
            var fromPassword = @"mssp.WXK60qK.351ndgwqopdgzqx8.c80h4tl";
            using MailMessage message = new();
            message.From = new MailAddress(fromEmail);
            message.To.Add(new MailAddress(email));
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;
            using SmtpClient smtpClient = new("smtp.mailersend.net")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };
            await smtpClient.SendMailAsync(message);
        }
    }
}