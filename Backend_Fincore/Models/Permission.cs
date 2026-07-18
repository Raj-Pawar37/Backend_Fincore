using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Permission : BaseEntity
    {
        [Key]
        public int PermissionId { get; set; }

        public string PermissionName { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public string? Description { get; set; }


        // Navigation property
        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}