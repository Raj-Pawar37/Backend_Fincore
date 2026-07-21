namespace Backend_Fincore.DTOs.APInvoice
{
    public class APInvoiceCUDTO
    {
        public int APInvoiceId { get; set; }

        public int VendorId { get; set; }

        public int MasterId { get; set; }

        public string MasterType { get; set; } = null!;

        public decimal InvoiceAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string Status { get; set; } = null!;

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
