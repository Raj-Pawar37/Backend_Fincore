using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Approval
{
    public class ApprovalWriteDTO
    {
        [Required]
        public decimal MinAmount { get; set; }

        [Required]
        public decimal MaxAmount { get; set; }

        [Required]
        public int ApprovalLevel { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
