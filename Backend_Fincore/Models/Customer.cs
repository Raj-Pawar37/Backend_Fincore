namespace Backend_Fincore.Models
{
    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }

        public int CompanyId { get; set; }

        public string CustomerName { get; set; } = null!;

        public string CustomerCode { get; set; } = null!;

        public string PANNo { get; set; } = null!;

        public string? Description { get; set; }

        // Navigation property
        public Company Company { get; set; } = null!;

        public ICollection<RevenueEntry> RevenueEntries { get; set; }
            = new List<RevenueEntry>();

        public ICollection<ARInvoice> ARInvoices { get; set; }
            = new List<ARInvoice>();
    }
}
