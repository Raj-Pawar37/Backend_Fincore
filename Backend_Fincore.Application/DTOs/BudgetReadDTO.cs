using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class BudgetReadDTO
    {
        public int BudgetId { get; set; }

        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public string FinancialYear { get; set; } = null!;

        public decimal TotalBudget { get; set; }

        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
