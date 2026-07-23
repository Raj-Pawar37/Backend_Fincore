using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Approval
{
    public class ApprovalReadDTO
    {
        public int ApprovalId { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public int ApprovalLevel { get; set; }

        public string RoleName { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
