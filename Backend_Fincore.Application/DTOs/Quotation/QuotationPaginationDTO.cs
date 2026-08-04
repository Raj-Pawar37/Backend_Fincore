using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Quotation
{
    public class QuotationPaginationDTO : PaginationDTO
    {
        public int? VendorId { get; set; }

        public string? Status { get; set; }
    }
}
