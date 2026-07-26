using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RevenueEntry
{
    public class RevenueEntryCreateDto
    {
        public int CustomerId { get; set; }

        public string ProfitCenterName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime RevenueDate { get; set; }

        public string Status { get; set; } = string.Empty;


        // Audit fields
        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
