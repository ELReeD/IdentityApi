using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StepApi.Identity.Data;
using StepApi.Identity.Model;
using StepApi.Identity.Options;
using StepApi.Identity.Services;

namespace StepApi.Identity.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,WebApplicationBuilder builder)
        {
            //Services
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
            //Services


            //JWt

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            //Jwt


            //DbContext
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));
            //DbContext

            //Identity
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<AuthDbContext>();
            //Identity


            //Logger
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


            Log.Logger = new LoggerConfiguration()
               .WriteTo.RollingFile(System.IO.Path.Combine("../logs/{Date}-logs.txt"))
               .CreateLogger();

            //Logger


            return services;
        }

    }
}
