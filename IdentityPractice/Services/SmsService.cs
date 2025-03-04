using IdentityPractice.Security;
using Microsoft.Extensions.Options;

namespace IdentityPractice.Srvices
{
    public class SmsService :ISmsService
    {
        private readonly KavenegarInfo _kavenegarInfo;

        public SmsService(IOptions<KavenegarInfo> kavenegarInfo)
        {
            _kavenegarInfo = kavenegarInfo.Value;
        }

        public async Task SendAsync(string PhoneNumber, string message)
        {

            try
            {
                var api = new Kavenegar.KavenegarApi(_kavenegarInfo.ApiKey);

                var result = await api.Send(_kavenegarInfo.Sender, PhoneNumber, message);
            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendLookupSMS(string phoneNumber, string templateName, string token1, string? token2 = "",
           string? token3 = "")
        {
            try
            {
                var api = new Kavenegar.KavenegarApi(_kavenegarInfo.ApiKey);

                var result = await api.VerifyLookup(phoneNumber, token1, token2, token3, templateName);
            }
            catch (Kavenegar.Core.Exceptions.ApiException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Kavenegar.Core.Exceptions.HttpException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool VerifyTotp(string code, string totpCode)
        {
            return code == totpCode ? true : false;
        }
    }
}
