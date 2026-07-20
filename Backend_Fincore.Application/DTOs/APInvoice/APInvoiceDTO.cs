namespace Backend_Fincore.DTOs.APInvoice
{
    public class APInvoiceDTO
    {
        public int APInvoiceId { get; set; }

        public int VendorId { get; set; }

        public string VendorName { get; set; } = null!;

        public int MasterId { get; set; }

        public string MasterType { get; set; } = null!;

        public string InvoiceNumber { get; set; } = null!;

        public decimal InvoiceAmount { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string Status { get; set; } = null!;
    }
}
