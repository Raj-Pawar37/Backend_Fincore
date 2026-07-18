namespace Backend_Fincore.Models
{
    public class UserToken : BaseEntity
    {
        public int UserTokenId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; } = null!;

        // RefreshToken / PasswordReset / EmailVerification
        public string TokenType { get; set; } = null!;

        public DateTime ExpiryDate { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}
