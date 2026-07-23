using Backend_Fincore.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class PurchaseOrderItem : BaseEntity
    {
        [Key]
        public int POItemId { get; set; }

        public int PurchaseOrderId { get; set; }

        public int QuotationItemId { get; set; }

        public string ItemName { get; set; } = null!;

        public string? ItemType { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? Tax { get; set; }

        public decimal? Discount { get; set; }

        public int Qty { get; set; }

        // New Field
        public string Status { get; set; } = "Pending";

        // Navigation Properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public QuotationItem QuotationItem { get; set; } = null!;

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();

        public GRNItem? GRNItem { get; set; }
    }
}