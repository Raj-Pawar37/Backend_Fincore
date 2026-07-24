namespace Backend_Fincore.Application.DTOs.RFQItem
{
    public class RFQItemResponseDto
    {
        public int RFQItemId { get; set; }
        public int RFQId { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public string? Description { get; set; }

        // public string? AttachmentPath { get; set; } 
    }
}