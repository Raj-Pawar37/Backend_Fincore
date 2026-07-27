using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Quotation
{
    public class QuotationCDTO
    {
        public int RFQId { get; set; }

        public int RFQVendorId { get; set; }

        public string QuotationNumber { get; set; } = string.Empty;

        public DateTime QuotationDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Desc { get; set; }
    }
}
