using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class BudgetLineDropdownDTO
    {
        public int BudgetLineId { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public decimal AllocatedAmount { get; set; }

        public decimal AvailableAmount { get; set; }
    }
}
