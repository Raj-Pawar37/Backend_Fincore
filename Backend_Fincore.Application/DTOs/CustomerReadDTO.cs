using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs
{
   public  class CustomerReadDTO
    {
        public int CustomerId { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string CustomerCode { get; set; } = null!;

        public string PANNo { get; set; } = null!;

        public string? Description { get; set; }
    }
}
