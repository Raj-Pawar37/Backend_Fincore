using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RevenueEntry
{
    public class RevenueEntryUpdateDto
    {
        public int RevenueEntryId { get; set; }

        public int CustomerId { get; set; }

        public string ProfitCenterName { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime RevenueDate { get; set; }

        public string? Status { get; set; }
    }
}
