using System.ComponentModel.DataAnnotations;

namespace StepApi.Identity.DTO
{
    public class RegistartionDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime BirthdayDate { get; set; }
        [Required]
        public DateTime RegistartionDate { get; set; }
        [Required]
        public string PhoneNumber{ get; set; }
    }
}
