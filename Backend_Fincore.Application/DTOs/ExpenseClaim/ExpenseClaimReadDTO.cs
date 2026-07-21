using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.ExpenseClaim
{
    public  class ExpenseClaimReadDTO
    {


        public int ExpenseClaimId { get; set; }

        public int OpexRequestId { get; set; }

        public string ClaimNumber { get; set; } = null!;

        public decimal ExpenseAmount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string? Description { get; set; }

        public string? BillFilePath { get; set; }

        public int ClaimedBy { get; set; }

        public string Status { get; set; } = null!;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
