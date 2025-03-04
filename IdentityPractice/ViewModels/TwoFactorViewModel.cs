using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class TwoFactorViewModel
    {
        [Required]
        [Display(Name ="شماره ثابت ")]
        [MinLength(11)]
        [MaxLength(11, ErrorMessage = "شماره باید 11 رقمی باشد")]
        public string HomePhoneNumber { get; set; }
    }
}
