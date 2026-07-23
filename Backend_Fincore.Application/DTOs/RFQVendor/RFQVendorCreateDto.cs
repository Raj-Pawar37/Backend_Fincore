using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs.RFQVendor
{
    public class RFQVendorCreateDto
    {
        [Required]
        public int RFQId { get; set; }

        [Required]
        public int VendorId { get; set; }

        [Required]
        public DateTime SentDate { get; set; }
    }
}