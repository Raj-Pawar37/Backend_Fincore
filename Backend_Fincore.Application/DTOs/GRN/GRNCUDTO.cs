namespace Backend_Fincore.DTOs.GRN
{
    public class GRNCUDTO
    {
        public int GRNId { get; set; }

        public int PurchaseOrderId { get; set; }

        public int ReceivedBy { get; set; }

        public DateTime ReceivedDate { get; set; }

        public string? Remarks { get; set; }

        public string? DeliveryChallanNumber { get; set; }

        public string Status { get; set; } = null!;

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
