using Microsoft.AspNetCore.Identity;
using StepApi.Identity.DTO;
using StepApi.Identity.Model;
using StepApi.Identity.Model.OutputModel;

namespace StepApi.Identity.Services
{
    public interface IAuthService
    {
        public Task<ResponseLogin> Login(LoginDTO loginDTO);
        public Task<ResponseRegistration> Registartion(RegistartionDTO registartionDTO);
        public Task<ResponseLogin> RefreshToken(string token);
        public Task Logout(string refreshToken);
    }
}
