namespace Backend_Fincore.Application.DTOs.Vendor
{
    public class VendorWriteDTO
    {
        public int CompanyId { get; set; }

        public string VendorName { get; set; } = null!;

        public string VendorCode { get; set; } = null!;

        public string PANNo { get; set; } = null!;

        public string? Description { get; set; }
    }
}