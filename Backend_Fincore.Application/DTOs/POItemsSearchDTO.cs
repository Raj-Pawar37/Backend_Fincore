using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public  class POItemsSearchDTO
    {

        public int POItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = null!;
    }
}
