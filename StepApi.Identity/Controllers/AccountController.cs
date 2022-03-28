using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StepApi.Identity.Data;
using StepApi.Identity.DTO;
using StepApi.Identity.Model;
using StepApi.Identity.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace StepApi.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AuthDbContext dbContext;
        private readonly UserManager<AppUser> userManager;

        public AccountController(IAuthService authService,RoleManager<IdentityRole> roleManager,AuthDbContext dbContext,UserManager<AppUser> userManager)
        {
            this.authService = authService;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registartion(RegistartionDTO registration)
        {
            var result = await authService.Registartion(registration);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await authService.Login(loginDTO);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> RefreshToken([Required]string token)
        {
            var result = await authService.RefreshToken(token);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Logout([Required] string refreshToken)
        {
            await authService.Logout(refreshToken);
            return Ok();
        }

        [HttpGet("PingAdmin")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        public IActionResult Ping()
        {
            return Ok();
        }

        [HttpGet("PingUser")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult PingUser()
        {
            var a = this.User.Identities;
            var b = a.First().Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok(b.Value);
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> CreateRoleAsync(string role)
        {
            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Admin"
            });

            return Ok(roleManager.Roles);
        }


        /*TEST ZONE!*/

        [HttpGet("Users")]
        public IActionResult GetUser()
        {
            var users = dbContext.Users;

            List<object> Test = new List<object>();

            foreach (var user in users)
            {             

                Test.Add(_ = (new
                {
                    userName = user.UserName,
                    id = user.Id
                }));
            }

            return Ok(Test);
        }

        [HttpGet("GetUserRole")]
        public IActionResult GetRole(string userId)
        {
            var roleId = dbContext.UserRoles.Where(x => x.UserId == userId);
            //var roleName = dbContext.Roles.FirstOrDefault  (x=>  x.Id == roleId.RoleId);


            return Ok(roleId);
        }

        [HttpGet("GetRole")]
        public IActionResult GetRoles()
        {
            var roles = dbContext.Roles;
            return Ok(roles);
        }

        [HttpGet("SetUserRole")]
        public async Task<IActionResult> SetRoleAsync(string userId,string roleId)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Id == userId);
            var role = dbContext.Roles.FirstOrDefault(x => x.Id == roleId);
            await userManager.AddToRoleAsync((AppUser)user,role.Name);

            return Ok();
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> crtRole(string role)
        {
            var res = await roleManager.CreateAsync(new IdentityRole { Name = role });
            if (res.Succeeded)
                return Ok("True!");

            return BadRequest("false");
        }
    }
}