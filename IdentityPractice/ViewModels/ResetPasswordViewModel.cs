using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "رمزعبور")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "تکرار رمزعبور")]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
