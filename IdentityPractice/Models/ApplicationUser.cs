using Microsoft.AspNetCore.Identity;

namespace IdentityPractice.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? City { get; set; }
        public string? HomePhoneNumber { get; set; }
    }
}
