using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class WorkOrder : BaseEntity
    {
        [Key]
        public int WorkOrderId { get; set; }

        public int OpexRequestId { get; set; }

        public string WorkOrderNumber { get; set; } = null!;

        public int VendorId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Draft / Issued / InProgress / Completed / Cancelled
        public string Status { get; set; } = null!;


        // Navigation properties
        public OpexRequest OpexRequest { get; set; } = null!;

        public Vendor Vendor { get; set; } = null!;
    }
}