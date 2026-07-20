using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class PurchaseRequisition : BaseEntity
    {
        [Key]
        public int PRId { get; set; }

        public int CapexRequestId { get; set; }

        public string PRNumber { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string Status { get; set; } = null!;


        // Navigation properties
        public CapexRequest CapexRequest { get; set; } = null!;

        public ICollection<RFQ> RFQs { get; set; }
            = new List<RFQ>();
    }
}