namespace StepApi.Identity.Model.OutputModel
{
    public class Token
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
