using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
    public class GRNItemsCUDTO
    {
        public int GRNItemId { get; set; }

        public int GRNId { get; set; }

        public int POItemId { get; set; }

        public string? Remarks { get; set; }

        public decimal Qty { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
