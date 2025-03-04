using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class VerifyTwoFactorViewModel
    {
      //  public string Token { get; set; }
        public string HomePhoneNumber { get; set; }
     
        [Display(Name ="کد تایید")]
        [Required]
        [MinLength(6)]
        [MaxLength(6,ErrorMessage ="کد شش رقمی است ")]
        public string Code { get; set; }
    }
}
