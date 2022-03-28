using System.ComponentModel.DataAnnotations;

namespace StepApi.Identity.DTO
{
    public class LoginDTO
    {
        [Required]
        public string Login{ get; set; }

        [Required]
        public string Password{ get; set; }
    }
}
