using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class BudgetLineReadDTO
    {
        public int BudgetLineId { get; set; }

        public int BudgetId { get; set; }

        public string? FinancialYear { get; set; }

        public string? CompanyName { get; set; }

        public string? DepartmentName { get; set; }

        public string CostCenter { get; set; } = null!;

        public int BudgetCategoryId { get; set; }

        public string? BudgetCategoryName { get; set; }

        public decimal AllocatedAmount { get; set; }
    }
}
