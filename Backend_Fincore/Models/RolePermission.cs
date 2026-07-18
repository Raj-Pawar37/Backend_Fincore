using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class RolePermission : BaseEntity
    {
        [Key]
        public int RolePermissionId { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }


        // Navigation properties
        public Role Role { get; set; } = null!;

        public Permission Permission { get; set; } = null!;

        public User CreatedByUser { get; set; } = null!;
    }
}