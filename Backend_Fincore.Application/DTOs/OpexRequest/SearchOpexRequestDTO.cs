using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.OpexRequest
{
    public class OpexSearchDTO
    {
        public string? Status { get; set; }

        public string? SearchText { get; set; }

        public string? Department { get; set; }
    }
}
