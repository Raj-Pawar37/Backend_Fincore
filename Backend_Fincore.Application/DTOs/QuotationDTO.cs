using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public class QuotationDTO
    {
        public int QuotationId { get; set; }

        public int RFQVendorId { get; set; }

        public string QuotationNumber { get; set; } = null!;

        public decimal Amount { get; set; }

        public string Status { get; set; } = null!;

        public string? AttachmentPath { get; set; }

        public string? Description { get; set; }
    }
}
