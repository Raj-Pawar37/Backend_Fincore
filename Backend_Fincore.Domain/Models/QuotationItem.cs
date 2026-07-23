using Backend_Fincore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Domain.Models
{
    public class QuotationItem : BaseEntity
    {
        public int QuotationItemId { get; set; }

        public int QuotationId { get; set; }

        public int RFQItemId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Tax { get; set; }

        public decimal Discount { get; set; }

        public string Status { get; set; } = "Pending";


        // Navigation properties

        public Quotation Quotation { get; set; } = null!;

        public RFQItem RFQItem { get; set; } = null!;
    }
}
