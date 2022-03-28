using Microsoft.AspNetCore.Identity;
using Serilog;
using StepApi.Identity.Data;
using StepApi.Identity.DTO;
using StepApi.Identity.Model;
using StepApi.Identity.Model.OutputModel;
using System.Collections.Generic;

namespace StepApi.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly AuthDbContext dbContext;
        private readonly UserManager<AppUser> userManager;

        public AuthService(ITokenGenerator tokenGenerator, AuthDbContext dbContext, UserManager<AppUser> userManager)
        {
            this.tokenGenerator = tokenGenerator;
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<ResponseLogin> Login(LoginDTO loginDTO)
        {
            var response = new ResponseLogin
            {
                Success = false
            };

            try
            {
                var user = await userManager.FindByEmailAsync(loginDTO.Login);

                if (user == null)
                {
                    response.Message = "Login or Password wrong";
                    return response;
                }

                var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

                if (!result)
                {
                    response.Message = "Login or Password wrong";
                    return response;
                }

                var token = tokenGenerator.CreateJwtToken(user);


                var list = dbContext.RefreshTokens.Where(x => x.AppUserId == user.Id).ToList();
                if (list.Count() > 0)
                {
                    dbContext.RefreshTokens.RemoveRange(list);
                }

                var refreshToken = new RefreshToken
                {
                    AppUser = user,
                    AppUserId = user.Id,
                    ExpiresAt = DateTime.UtcNow + TimeSpan.FromDays(30),
                    Token = tokenGenerator.CreateRefreshToken()
                };


                await dbContext.RefreshTokens.AddAsync(refreshToken);
                await dbContext.SaveChangesAsync();


                response.Success = true;
                response.Message = ResponseMessage.Success.ToString();
                response.Token = new Token
                {
                    RefreshToken = refreshToken.Token,
                    JwtToken = token,
                    UserId = user.Id,
                    Username = user.UserName
                };

                return response;

            }
            catch (Exception ex)
            {
                response.Message = ResponseMessage.Error.ToString();
                response.Success = false;
                Log.Logger.Error($"Login Service : {ex.Message}");
                return response;
            }

        }

        public async Task<ResponseRegistration> Registartion(RegistartionDTO registartionDTO)
        {
            var response = new ResponseRegistration
            {
                Success = false,
                Message = ResponseMessage.Error.ToString()
            };

            try
            {

                var user = new AppUser
                {
                    FullName = registartionDTO.FullName,
                    Email = registartionDTO.Email,
                    UserName = registartionDTO.Email,
                    RegistartionDate = DateTime.UtcNow,
                    BirthdayDate = registartionDTO.BirthdayDate,
                    PhoneNumber = registartionDTO.PhoneNumber
                };



                var result = await userManager.CreateAsync(user, registartionDTO.Password);

                if (!result.Succeeded)
                {
                    response.Errors = result.Errors;
                    return response;
                }


                response.Message = ResponseMessage.Success.ToString();
                response.Success = true;

                return response;
            }
            catch (Exception ex)
            {
                response.Message = ResponseMessage.Error.ToString();
                response.Success = false;

                Log.Logger.Error($"Registartion Service : {ex.Message}");
                return response;
            }

        }

        public async Task<ResponseLogin> RefreshToken(string token)
        {
            ResponseLogin response = new ResponseLogin
            {
                Message = ResponseMessage.Error.ToString(),
                Success = false
            };

            try
            {
                var refreshToken = dbContext.RefreshTokens.FirstOrDefault(x => x.Token == token);

                if (refreshToken == null)
                    return response;


                var user = dbContext.Users.FirstOrDefault(x => x.Id == refreshToken.AppUserId);

                if (refreshToken != null && refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    dbContext.RefreshTokens.Remove(refreshToken);
                    await dbContext.SaveChangesAsync();

                    return response;
                }

                var newToken = new RefreshToken()
                {
                    Token = tokenGenerator.CreateRefreshToken(),
                    AppUserId = user.Id,
                    AppUser = user as AppUser,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                await dbContext.RefreshTokens.AddAsync(newToken);
                dbContext.RefreshTokens.Remove(refreshToken);
                await dbContext.SaveChangesAsync();



                response.Success = true;
                response.Message = ResponseMessage.Success.ToString();
                response.Token = new Token
                {
                    RefreshToken = newToken.Token,
                    JwtToken = tokenGenerator.CreateJwtToken((AppUser)user),
                    UserId = user.Id,
                    Username = user.UserName
                };

                return response;
            }
            catch (Exception ex)
            {
                response.Message = ResponseMessage.Error.ToString();
                response.Success = false;

                Log.Logger.Error($"Registartion Service : {ex.Message}");
                return response;
            }
        }

        public async Task Logout(string refreshToken)
        {
            var token = dbContext.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
            dbContext.RefreshTokens.Remove(token);
            await dbContext.SaveChangesAsync();
        }     
    }
}
