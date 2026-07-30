using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.OpexRequest
{
    public  class OpexRequestDropdownDTO
    {
        public int OpexRequestId { get; set; }
        public string Title { get; set; } = null!;
    }
}
