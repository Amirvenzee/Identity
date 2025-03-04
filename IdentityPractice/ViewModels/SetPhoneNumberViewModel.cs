using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class SetPhoneNumberViewModel
    {
        [Required]
        [RegularExpression(@"(\+98|0)?9\d{9}",ErrorMessage ="لطفا شماره تلفن ایرانی وارد کن ")]
        [MinLength(11)]
        [MaxLength(11,ErrorMessage ="شماره باید 11 رقمی باشد")]
        
        public string PhoneNumber { get; set; }
    }
}
