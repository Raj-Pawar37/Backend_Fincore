using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs.RFQVendor
{
    public class RFQVendorUpdateDto
    {
        [Required]
        public int RFQId { get; set; }

        [Required]
        public int VendorId { get; set; }

        public string? ResponseStatus { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}