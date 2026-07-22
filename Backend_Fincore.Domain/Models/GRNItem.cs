using Backend_Fincore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Domain.Models
{
    public class GRNItem
    {
        public int GRNItemId { get; set; }

        public int GRNId { get; set; }

        public int POItemId { get; set; }

        public string? Remarks { get; set; }

        public decimal Qty { get; set; }


        // Navigation properties
        public GRN GRN { get; set; } = null!;

        public PurchaseOrderItem POItem { get; set; } = null!;
    }
}
