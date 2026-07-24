using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs.RFQ
{
    public class RFQUpdateDto
    {
        [Required]
        public string RFQNumber { get; set; } = null!;
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public DateTime IssueDate { get; set; }
        [Required]
        public DateTime ClosingDate { get; set; }
        public string? Status { get; set; }
    }
}