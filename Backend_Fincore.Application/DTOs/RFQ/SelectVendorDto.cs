using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RFQ
{
   public  class SelectVendorDto
    {
        public int QuotationId { get; set; }
        public string? Justification { get; set; }

    }
}
