using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RFQ
{
    public class RFQDropdownDto
    {
        public int RFQId { get; set; }

        public string RFQNumber { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
