namespace Backend_Fincore.DTOs.PurchaseOrder
{
    public class PurchaseOrderDTO
    {
        public int PurchaseOrderId { get; set; }

        public int VendorId { get; set; }

        public int QuotationId { get; set; }

        public string PONumber { get; set; }

        public decimal TotalAmount { get; set; }

       

        public string Status { get; set; }

        //public string VendorName { get; set; }

        //public string QuotationNumber { get; set; }
    }
}
