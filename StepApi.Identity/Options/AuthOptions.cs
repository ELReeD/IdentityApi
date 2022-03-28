using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StepApi.Identity.Options
{
    public static class AuthOptions
    {
        public static readonly string ISSUER = "StepApiIdentityServer";
        public static readonly string AUDIENCE = "StepProjectClient";
        public static readonly string KEY = "04c9b053-e904-4d93-9b70-54d982655e84";
        public static readonly int LIFETIME = 10;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
