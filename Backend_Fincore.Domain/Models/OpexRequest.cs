
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class OpexRequest : BaseEntity
    {
        [Key]
        public int OpexRequestId { get; set; }

        public int BudgetLineId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }

        // Pending / Approved / Rejected
        public string Status { get; set; } = null!;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }


        // Navigation properties
        public BudgetLine BudgetLine { get; set; } = null!;

        public User RequestedByUser { get; set; } = null!;

        public User? ApprovedByUser { get; set; }

        public ICollection<ExpenseClaim> ExpenseClaims { get; set; }
            = new List<ExpenseClaim>();

        public ICollection<WorkOrder> WorkOrders { get; set; }
            = new List<WorkOrder>();
    }
}