using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class ExternalLoginCallBackViewModel
    {
        [Required]
        [Display(Name = "نام کاربری")]
        [Remote("IsUserNameInUse", "Account", HttpMethod = "POST",
      AdditionalFields = "__RequestVerificationToken")]
        public string UserName { get; set; }

        [Display(Name = "رمزعبور")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage =" رمز عبور باید حداقل 6 کاراکتر باشد")]
        public string? Password { get; set; }

        [Display(Name = "تکرار رمزعبور")]
        [Compare(nameof(Password),ErrorMessage ="رمز عبور و تکرار رمز عبور یکسان نیستند")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = " تکرار رمز عبور باید حداقل 6 کاراکتر باشد")]
        public string? ConfirmPassword { get; set; }
    }
}
