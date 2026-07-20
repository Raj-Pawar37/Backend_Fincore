using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class CapexRequest : BaseEntity
    {
        [Key]
        public int CapexRequestId { get; set; }

        public int BudgetLineId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }

        public string Status { get; set; } = null!;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }


        // Navigation properties
        public BudgetLine BudgetLine { get; set; } = null!;

        public User RequestedByUser { get; set; } = null!;

        public User? ApprovedByUser { get; set; }

        public ICollection<PurchaseRequisition> PurchaseRequisitions { get; set; }
            = new List<PurchaseRequisition>();
    }
}