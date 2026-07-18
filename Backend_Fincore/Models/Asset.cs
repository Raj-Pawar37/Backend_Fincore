using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Asset : BaseEntity
    {
        [Key]
        public int AssetId { get; set; }

        public int GRNId { get; set; }

        public int POItemId { get; set; }

        public string AssetCode { get; set; } = null!;

        public string AssetName { get; set; } = null!;

        public decimal PurchaseCost { get; set; }

        public DateTime PurchaseDate { get; set; }

        // Available / Assigned / Retired
        public string Status { get; set; } = null!;

        public int? AssignedTo { get; set; }


        // Navigation properties
        public GRN GRN { get; set; } = null!;

        public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;

        public User? AssignedToUser { get; set; }
    }
}