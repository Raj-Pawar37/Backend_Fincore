using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.GRN
{
    public class GrnStatusDTO
    {
        public int userId { get; set; }
        public string Status { get; set; } = null!;
    }
}
