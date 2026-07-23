using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class CapexWriteDTO
    {
        public int BudgetLineId { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }
    }
}
