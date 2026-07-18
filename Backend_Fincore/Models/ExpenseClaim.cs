using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class ExpenseClaim : BaseEntity
    {
        [Key]
        public int ExpenseClaimId { get; set; }

        public int OpexRequestId { get; set; }

        public string ClaimNumber { get; set; } = null!;

        public decimal ExpenseAmount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string? Description { get; set; }

        public string? BillFilePath { get; set; }

        public int ClaimedBy { get; set; }

        // Pending / Approved / Rejected / Paid
        public string Status { get; set; } = null!;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }


        // Navigation properties
        public OpexRequest OpexRequest { get; set; } = null!;

        public User ClaimedByUser { get; set; } = null!;

        public User? ApprovedByUser { get; set; }
    }
}