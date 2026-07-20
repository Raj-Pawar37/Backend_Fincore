namespace Backend_Fincore.Models
{
    public class Vendor : BaseEntity
    {
        public int VendorId { get; set; }

        public int CompanyId { get; set; }

        public string VendorName { get; set; } = null!;

        public string VendorCode { get; set; } = null!;

        public string PANNo { get; set; } = null!;

        public string? Description { get; set; }

        // Navigation property
        public Company Company { get; set; } = null!;

        public ICollection<RFQVendor> RFQVendors { get; set; }
            = new List<RFQVendor>();


        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
            = new List<PurchaseOrder>();

        public ICollection<APInvoice> APInvoices { get; set; }
            = new List<APInvoice>();

        public ICollection<WorkOrder> WorkOrders { get; set; }
            = new List<WorkOrder>();
    }
}
