using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class BudgetLineWriteDTO
    {
        public int BudgetId { get; set; }

        public string CostCenter { get; set; } = null!;

        public int BudgetCategoryId { get; set; }

        public decimal AllocatedAmount { get; set; }
    }
}