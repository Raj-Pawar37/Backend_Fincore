
using Backend_Fincore.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Quotation : BaseEntity
    {
        [Key]
        public int QuotationId { get; set; }

        public int RFQVendorId { get; set; }

        public string QuotationNumber { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Status { get; set; } = null!;

        public string? AttachmentPath { get; set; }

        public string? Description { get; set; }


        // Navigation property
        public RFQVendor RFQVendor { get; set; } = null!;

        public PurchaseOrder? PurchaseOrder { get; set; }

        public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();

    }
}