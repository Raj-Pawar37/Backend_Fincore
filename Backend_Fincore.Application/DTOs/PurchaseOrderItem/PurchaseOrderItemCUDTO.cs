namespace Backend_Fincore.DTOs.PurchaseOrderItem
{
    public class PurchaseOrderItemCUDTO
    {

        public int POItemId { get; set; }

        public int PurchaseOrderId { get; set; }

        public int QuotationItemId { get; set; }

        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Tax { get; set; }

        public decimal Discount { get; set; }

        public int Qty { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
