namespace IdentityPractice.Repositories
{
    public interface IMessageSender
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
        public  Task<string> GetEmailBodyAsync(string userName, string confirmLink);
    }
}
