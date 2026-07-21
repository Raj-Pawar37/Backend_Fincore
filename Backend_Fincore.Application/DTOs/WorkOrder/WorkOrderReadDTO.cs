using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.WorkOrder
{
    public class WorkOrderReadDTO
    {
        public int WorkOrderId { get; set; }

        public int OpexRequestId { get; set; }

        public string WorkOrderNumber { get; set; } = null!;

        public int VendorId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = null!;
    }
}
