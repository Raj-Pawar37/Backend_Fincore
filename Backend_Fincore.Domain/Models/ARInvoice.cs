using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class ARInvoice : BaseEntity
    {
        [Key]
        public int ARInvoiceId { get; set; }

        public int CustomerId { get; set; }

        public int RevenueEntryId { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public decimal InvoiceAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string Status { get; set; } = null!;

        public string? PONumber { get; set; }

        // Navigation properties
        public Customer Customer { get; set; } = null!;

        public RevenueEntry RevenueEntry { get; set; } = null!;
    }
}