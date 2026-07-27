using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.DTOs
{
    public class RolePermissionDTO
    {
        public int RolePermissionId { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public bool IsActive { get; set; }
    }

    public class RolePermissionResponseDTO
    {
        public int RolePermissionId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}