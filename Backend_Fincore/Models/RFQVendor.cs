using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class RFQVendor : BaseEntity
    {
        [Key]
        public int RFQVendorId { get; set; }

        public int RFQId { get; set; }

        public int VendorId { get; set; }

        public DateTime SentDate { get; set; }

        public string ResponseStatus { get; set; } = null!;

        public DateTime? ResponseDate { get; set; }


        // Navigation properties
        public RFQ RFQ { get; set; } = null!;

        public Vendor Vendor { get; set; } = null!;

        public ICollection<Quotation> Quotations { get; set; }
            = new List<Quotation>();
    }
}