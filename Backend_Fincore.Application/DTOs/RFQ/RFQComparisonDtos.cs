using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RFQ
{
    public class RFQComparisonDtos
    {
        // Represents a single vendor's quote in the comparison matrix
        public class QuotationComparisonDto
        {
            public int QuotationId { get; set; }
            public int VendorId { get; set; }
            public string VendorName { get; set; } = null!;
            public string QuotationNumber { get; set; } = null!;
            public decimal Amount { get; set; }
            public string? Description { get; set; }
            public string Status { get; set; } = null!;
            public DateTime? ResponseDate { get; set; }
        }

        // The main wrapper for the RFQ Comparison View
        public class RFQComparisonResponseDto
        {
            public int RFQId { get; set; }
            public string RFQNumber { get; set; } = null!;
            public string Title { get; set; } = null!;
            public DateTime ClosingDate { get; set; }

            // This array will hold the actual comparison data
            public List<QuotationComparisonDto> VendorQuotations { get; set; } = new List<QuotationComparisonDto>();
        }
    }
}
