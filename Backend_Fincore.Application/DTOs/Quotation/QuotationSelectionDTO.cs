using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Quotation
{
    public class QuotationSelectionDTO
    {
        public int RFQId { get; set; }
        public List<int> SelectedQuotationItemIds { get; set; } = new();
    }
}
