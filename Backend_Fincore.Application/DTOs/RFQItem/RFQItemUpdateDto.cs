using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs.RFQItem
{
    public class RFQItemUpdateDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        public string? Description { get; set; }

        // Rule: attachPath comment out for now
        // public string? AttachmentPath { get; set; } 
    }
}