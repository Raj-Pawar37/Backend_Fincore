using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class BudgetWriteDTO
    {
        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public string FinancialYear { get; set; } = null!;

        public decimal TotalBudget { get; set; }
    }
}
