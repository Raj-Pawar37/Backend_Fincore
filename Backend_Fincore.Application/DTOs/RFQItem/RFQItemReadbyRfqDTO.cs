using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RFQItem
{
    public class RFQItemReadbyRfqDTO
    {
        public int? RFQId { get; set; }
        public string? searchText { get; set; } = "";
    }
}
