namespace Backend_Fincore.Application.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public int? try12{get;set;}=null;
    }

    public class TokenRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}