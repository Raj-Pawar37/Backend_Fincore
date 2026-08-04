using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.PurchaseOrderItem
{
    public class SearchPoiDTO
    {

        public int PurchaseOrderId { get; set; }

        public string? Status { get; set; }

        public string? SearchText { get; set; }
    }
}
