using Backend_Fincore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Domain.Models
{
    public class Approval: BaseEntity
    {
        [Key]
        public int ApprovalId { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public int ApprovalLevel { get; set; }

        // Foreign Key
        public int RoleId { get; set; }

        // Navigation Property
        public Role Role { get; set; } = null!;
    }
}
