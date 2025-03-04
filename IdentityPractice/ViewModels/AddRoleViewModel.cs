using System.ComponentModel.DataAnnotations;

namespace IdentityPractice.ViewModels
{
    public class AddRoleViewModel
    {
        [Required(ErrorMessage ="اجباری است")]
        [Display(Name="مقام")]
        public string Name { get; set; }
    }
}
