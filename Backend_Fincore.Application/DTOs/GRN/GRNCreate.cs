using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.GRN
{
    public class GRNCreate
    {
        //public int GRNId { get; set; }
        public int PurchaseOrderId { get; set; }

        public string GRNNumber { get; set; } = null!;

        public int ReceivedBy { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string? Remarks { get; set; }

        public string? DeliveryChallanNumber { get; set; }

    }
}
