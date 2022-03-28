using StepApi.Identity.Model;

namespace StepApi.Identity.Services
{
    public interface ITokenGenerator
    {
        public string CreateJwtToken(AppUser user);
        public string CreateRefreshToken();
    }
}
