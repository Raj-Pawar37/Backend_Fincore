using Backend_Fincore.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Role : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public string RoleCode { get; set; } = null!;

        public string? RoleDescription { get; set; }


        // Navigation properties
        public ICollection<User> Users { get; set; }
            = new List<User>();

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
        public ICollection<Approval> Approvals { get; set; }
            = new List<Approval>();
    }
}