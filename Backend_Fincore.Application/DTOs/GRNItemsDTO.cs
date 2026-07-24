using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public class GRNItemsDTO
    {

        public int GRNItemId { get; set; }

        public int GRNId { get; set; }

        public int POItemId { get; set; }

        public string ItemName { get; set; } = null!;

        public string? ItemType { get; set; }

        public decimal OrderedQty { get; set; }

        public decimal ReceivedQty { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? Tax { get; set; }

        public decimal? Discount { get; set; }

        public string? Remarks { get; set; }
    }
}
