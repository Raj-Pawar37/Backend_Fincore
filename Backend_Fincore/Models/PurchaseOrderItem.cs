using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class PurchaseOrderItem : BaseEntity
    {
        [Key]
        public int POItemId { get; set; }

        public int PurchaseOrderId { get; set; }

        public string ItemName { get; set; } = null!;

        public string? ItemType { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? Tax { get; set; }

        public decimal? Discount { get; set; }

        public int Qty { get; set; }


        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public ICollection<Asset> Assets { get; set; }
            = new List<Asset>();
    }
}