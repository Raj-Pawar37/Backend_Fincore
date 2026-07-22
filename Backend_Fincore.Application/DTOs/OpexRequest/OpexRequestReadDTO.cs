using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.OpexRequest
{
    public class OpexRequestReadDTO
    {
        public int OpexRequestId { get; set; }

        public int BudgetLineId { get; set; }

        public string? BudgetLineName { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }

        public string? RequestedByName { get; set; }

        public string Status { get; set; } = null!;

        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
