using System.ComponentModel.DataAnnotations;

namespace StepApi.Identity.Model
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string AppUserId{ get; set; }
        public AppUser AppUser{ get; set; }
    }
}
