using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.DTOs
{
    public class RolePermissionDTOs
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public int? CreatedByUserId { get; set; }
    }

    public class RolePermissionResponseDto
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
    }
}