using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs.RFQ
{
    public class RFQCreateDto
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
        [Required]
        public int PRId { get; set; }
    }
}