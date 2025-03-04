using System.ComponentModel.DataAnnotations;

namespace identityPractice.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required( ErrorMessage ="ایمیل خود را وارد کنید")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
