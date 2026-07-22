using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.RFQ
{
    public class RFQItemCreateDto
    {
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? AttachmentPath { get; set; }
    }

    // Main RFQ Creation
    public class RFQCreateDto
    {
        public int PRId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ClosingDate { get; set; }

        // Nested items and vendors
        public List<RFQItemCreateDto> RFQItems { get; set; } = new List<RFQItemCreateDto>();
        public List<int> VendorIds { get; set; } = new List<int>();
    }

    // Response DTO
    public class RFQResponseDto
    {
        public int RFQId { get; set; }
        public int PRId { get; set; }
        public string RFQNumber { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
