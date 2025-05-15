
using System.Net.Mail;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace IdentityPractice.Repositories
{
    public class MessageSender : IMessageSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
        {
            using (var client = new SmtpClient())
            {

                var credentials = new NetworkCredential()
                {
                    UserName = "Amir.pm876@gmail.com",
                    Password = "dsoqrtplvgexhosd"
                };

                client.Credentials = credentials;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.EnableSsl = true;

                using var emailMessage = new MailMessage()
                {
                    To = { new MailAddress(toEmail) },
                    From = new MailAddress("Amir.pm876@gmail.com"), // with @gmail.com
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = isMessageHtml
                };

                client.Send(emailMessage);
            }

            return Task.CompletedTask;
        }

        public async Task<string> GetEmailBodyAsync(string userName, string confirmLink)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailConfirmation.html");
            var html = await File.ReadAllTextAsync(filePath);
            html = html.Replace("{{UserName}}", userName)
                       .Replace("{{ConfirmLink}}", confirmLink);
            return html;
        }



    }
}
