using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace IdentityPractice.ViewModels
{
    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "نام کاربری")]
        [Remote("IsUserNameInUse", "Login", HttpMethod = "POST",
       AdditionalFields = "__RequestVerificationToken")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "ایمیل")]
        [EmailAddress]
        [Remote("IsEmailInUse", "Login", HttpMethod = "POST",
          AdditionalFields = "__RequestVerificationToken")]
        public string Email { get; set; }


        [Required]
        [Display(Name ="شماره تلفن")]
        [RegularExpression(@"(\+98|0)?9\d{9}", ErrorMessage = "لطفا شماره تلفن ایرانی وارد کن ")]
        [MinLength(11)]
        [MaxLength(11, ErrorMessage = "شماره باید 11 رقمی باشد")]
        public string PhoneNUmber { get; set; }

        [Required]
        [Display(Name = "رمزعبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "تکرار رمزعبور")]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
