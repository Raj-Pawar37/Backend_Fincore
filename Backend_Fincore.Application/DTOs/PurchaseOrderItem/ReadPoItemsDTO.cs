using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.PurchaseOrderItem
{
    public class ReadPoItemsDTO
    {
        public int userId { get; set; }

        public int poItemId { get; set; }
    }
}
