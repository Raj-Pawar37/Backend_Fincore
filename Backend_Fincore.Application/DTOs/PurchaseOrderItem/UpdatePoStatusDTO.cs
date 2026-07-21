using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.PurchaseOrderItem
{
    public class UpdatePoStatusDTO
    {
        public string Status { get; set; } = null!;

        public int ModifiedBy { get; set; }
    }
}
