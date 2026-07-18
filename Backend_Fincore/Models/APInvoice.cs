using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class APInvoice : BaseEntity
    {
        [Key]
        public int APInvoiceId { get; set; }

        public int VendorId { get; set; }

        // PurchaseOrderId or WorkOrderId
        public int MasterId { get; set; }

        // PurchaseOrder / WorkOrder
        public string MasterType { get; set; } = null!;

        public string InvoiceNumber { get; set; } = null!;

        public decimal InvoiceAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        // Pending / Approved / Paid / Rejected
        public string Status { get; set; } = null!;


        // Navigation property
        public Vendor Vendor { get; set; } = null!;
    }
}