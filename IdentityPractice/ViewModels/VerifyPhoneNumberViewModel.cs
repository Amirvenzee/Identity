using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class VerifyPhoneNumberViewModel
    {
        public string PhoneNumber { get; set; }

        public string Totp { get; set; }
        [Required]
        [MinLength(6)]
        [MaxLength(6)]
        public string Code { get; set; }
    }
}
