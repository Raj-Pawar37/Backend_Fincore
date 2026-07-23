using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class CapexReadDTO
    {
        public int CapexRequestId { get; set; }

        public int BudgetLineId { get; set; }

        public string CostCenter { get; set; } = string.Empty;

        public string BudgetCategoryName { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string FinancialYear { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }

        public string RequestedByName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
