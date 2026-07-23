using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.ExpenseClaim
{
    public  class ExpenseClaimVerifyDTO
    {
        public string? Status {  get; set; }

        public int? BudgetLineId { get; set; }
    }
}
