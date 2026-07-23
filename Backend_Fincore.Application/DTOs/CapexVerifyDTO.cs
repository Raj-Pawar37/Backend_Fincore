using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.DTOs
{
    public class CapexVerifyDTO
    {
        public int CapexRequestId { get; set; }

        public int UserId { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
