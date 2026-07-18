namespace Backend_Fincore.DTOs.PurchaseOrder
{
    public class PurchaseOrderCUDTO
    {
        public int PurchaseOrderId { get; set; }

        public int VendorId { get; set; }

        public int QuotationId { get; set; }

        public string PONumber { get; set; }

        public DateTime PODate { get; set; }

        public string Status { get; set; }
    }
}
