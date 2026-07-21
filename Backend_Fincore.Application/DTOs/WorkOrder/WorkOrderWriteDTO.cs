using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.WorkOrder
{
    public class WorkOrderWriteDTO
    {
        [Required]
        public int OpexRequestId { get; set; }

        [Required]
        public string WorkOrderNumber { get; set; } = null!;

        [Required]
        public int VendorId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
