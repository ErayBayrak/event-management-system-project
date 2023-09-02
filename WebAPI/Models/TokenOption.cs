namespace WebAPI.Models
{
    public class TokenOption
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public string Token { get; set; }
    }
}
