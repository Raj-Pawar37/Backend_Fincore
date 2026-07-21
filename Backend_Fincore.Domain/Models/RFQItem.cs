
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class RFQItem : BaseEntity
    {
        [Key]
        public int RFQItemId { get; set; }

        public int RFQId { get; set; }

        public string Name { get; set; } = null!;

        public int Quantity { get; set; }

        public string? Description { get; set; }

        public string? AttachmentPath { get; set; }


        // Navigation property
        public RFQ RFQ { get; set; } = null!;

       

    }
}