using System.ComponentModel.DataAnnotations;

namespace IdentityPractice.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage ="{0} اجباری است")]
        [Display( Name = "نام کاربری")]
        public string Name { get; set; }

        [Display(Name="شهر")]
        [Required(ErrorMessage =" {0} اجباری است")]
        public string City { get; set; }

       

    }
}
