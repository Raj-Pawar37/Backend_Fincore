using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.DTOs
{
    public class UserWriteDTO
    {
        public int RoleId { get; set; }

        public int MasterId { get; set; }

        [Required]
        public string MasterType { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? MobileNo { get; set; }

        public int FailedLoginAttempts { get; set; }

        public byte IsEmailVerified { get; set; }

        public DateTime? LastLoginDate { get; set; }
    }
}