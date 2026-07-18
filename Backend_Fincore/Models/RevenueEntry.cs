using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class RevenueEntry : BaseEntity
    {
        [Key]
        public int RevenueEntryId { get; set; }

        public int CustomerId { get; set; }

        public string ProfitCenterName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime RevenueDate { get; set; }

        public string Status { get; set; } = null!;


        // Navigation properties
        public Customer Customer { get; set; } = null!;

        public ICollection<ARInvoice> ARInvoices { get; set; }
            = new List<ARInvoice>();
    }
}