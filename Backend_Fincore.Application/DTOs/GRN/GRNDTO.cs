namespace Backend_Fincore.DTOs.GRN
{
    public class GRNDTO
    {
        public int GRNId { get; set; }

        public int PurchaseOrderId { get; set; }

        public string PONumber { get; set; } = null!;   

        public string GRNNumber { get; set; } = null!;

        public int ReceivedBy { get; set; }

        public string Username { get; set; } = null!;

        public DateTime ReceivedDate { get; set; }

        public string? Remarks { get; set; }

        public string? DeliveryChallanNumber { get; set; }

        public string Status { get; set; } = null!;
    }
}
