using System;

namespace Backend_Fincore.Application.DTOs.RFQVendor
{
    public class RFQVendorResponseDto
    {
        public int RFQVendorId { get; set; }
        public int RFQId { get; set; }
        public int VendorId { get; set; }
        public DateTime SentDate { get; set; }
        public string ResponseStatus { get; set; } = null!;
        public DateTime? ResponseDate { get; set; }
    }
}