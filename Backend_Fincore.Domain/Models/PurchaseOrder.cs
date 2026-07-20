using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class PurchaseOrder : BaseEntity
    {
        [Key]
        public int PurchaseOrderId { get; set; }

        public int VendorId { get; set; }

        public int QuotationId { get; set; }

        public string PONumber { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        // Draft / Issued / Completed
        public string Status { get; set; } = null!;


        // Navigation properties
        public Vendor Vendor { get; set; } = null!;

        public Quotation Quotation { get; set; } = null!;

        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
            = new List<PurchaseOrderItem>();

        public ICollection<GRN> GRNs { get; set; }
            = new List<GRN>();
    }
}