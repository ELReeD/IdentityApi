using Microsoft.AspNetCore.Identity;

namespace StepApi.Identity.Model
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime BirthdayDate { get; set; }
        public DateTime RegistartionDate { get; set; }
    }
}
