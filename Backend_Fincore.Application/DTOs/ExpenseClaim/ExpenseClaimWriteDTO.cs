using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.ExpenseClaim
{
    public  class ExpenseClaimWriteDTO
    {
        [Required]
        public int OpexRequestId { get; set; }

        [Required]

        public string ClaimNumber { get; set; }

        [Required]
     
        public decimal ExpenseAmount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

     
        public string? Description { get; set; }

        public string? BillFilePath { get; set; }

        [Required]
        public int ClaimedBy { get; set; }
    }
}
