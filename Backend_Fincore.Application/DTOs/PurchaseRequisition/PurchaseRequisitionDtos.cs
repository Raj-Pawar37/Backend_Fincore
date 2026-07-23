namespace Backend_Fincore.Application.DTOs.PurchaseRequisition
{
    // 1. Used when creating a new PR
    public class PurchaseRequisitionCreateDto
    {
        public int CapexRequestId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

    // 2. Used when updating a Draft PR
    public class PurchaseRequisitionUpdateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public string? Status { get; set; }
    }

    // 3. Used for sending data back to the frontend
    public class PurchaseRequisitionResponseDto
    {
        public int PRId { get; set; }
        public int CapexRequestId { get; set; }
        public string PRNumber { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
    }
}