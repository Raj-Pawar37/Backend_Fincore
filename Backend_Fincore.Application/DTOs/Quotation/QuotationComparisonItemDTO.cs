using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Quotation
{
    public class QuotationComparisonItemDTO
    {
        public int RFQItemId { get; set; }

        public string ItemName { get; set; } = null!;

        public decimal RequiredQuantity { get; set; }


        public int QuotationItemId { get; set; }

        public int QuotationId { get; set; }

        public string QuotationNumber { get; set; } = null!;


        public int VendorId { get; set; }

        public string VendorName { get; set; } = null!;


        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Tax { get; set; }

        public decimal Discount { get; set; }

        public decimal TotalAmount { get; set; }


        public string QuotationStatus { get; set; } = null!;

        public string ItemStatus { get; set; } = null!;
    }
}
