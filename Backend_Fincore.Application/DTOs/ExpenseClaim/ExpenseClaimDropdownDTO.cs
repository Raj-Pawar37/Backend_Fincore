using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.ExpenseClaim
{
    public class ExpenseClaimDropdownDTO
    {
        public int ExpenseClaimId { get; set; }
        public string ClaimNumber { get; set; } = null!;
    }
}
