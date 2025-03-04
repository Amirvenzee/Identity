namespace IdentityPractice.Srvices
{
    public interface ISmsService
    {
        public Task SendAsync(string PhoneNumber, string message);
        public Task SendLookupSMS(string phoneNumber, string templateName, string token1, string? token2 = "",
           string? token3 = "");

        public bool VerifyTotp(string code,string totpCode);
    }
}
