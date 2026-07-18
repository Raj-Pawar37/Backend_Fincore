using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class GRN : BaseEntity
    {
        [Key]
        public int GRNId { get; set; }

        public int PurchaseOrderId { get; set; }

        public string GRNNumber { get; set; } = null!;

        public int ReceivedBy { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string? Remarks { get; set; }

        public string? DeliveryChallanNumber { get; set; }

        // Received / Rejected
        public string Status { get; set; } = null!;


        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!;

        public User ReceivedByUser { get; set; } = null!;

        public ICollection<Asset> Assets { get; set; }
            = new List<Asset>();
    }
}