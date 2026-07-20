namespace Backend_Fincore.DTOs
{
    public class VendorReadDTO
    {
        public int VendorId { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string VendorName { get; set; } = null!;

        public string VendorCode { get; set; } = null!;

        public string PANNo { get; set; } = null!;

        public string? Description { get; set; }
    }
}