using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class RFQ : BaseEntity
    {
        [Key]
        public int RFQId { get; set; }

        public int PRId { get; set; }

        public string RFQNumber { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime ClosingDate { get; set; }

        public string Status { get; set; } = null!;


        // Navigation properties
        public PurchaseRequisition PurchaseRequisition { get; set; } = null!;

        public ICollection<RFQVendor> RFQVendors { get; set; }
            = new List<RFQVendor>();

        public ICollection<RFQItem> RFQItems { get; set; }
            = new List<RFQItem>();
    }
}