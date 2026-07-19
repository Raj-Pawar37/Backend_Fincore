namespace Backend_Fincore.DTOs
{
    public class UserReadDTO
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public int MasterId { get; set; }

        public string MasterType { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? MobileNo { get; set; }

        public int FailedLoginAttempts { get; set; }

        public byte IsEmailVerified { get; set; }

        public DateTime? LastLoginDate { get; set; }
    }
}