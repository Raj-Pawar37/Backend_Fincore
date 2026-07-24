namespace Backend_Fincore.Application.DTOs.PurchaseRequisition
{
    

    public class PurchaseRequisitionUpdateDto
    {
        public string? Title { get; set; }
        public string? PRNumber { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }

    public class PurchaseRequisitionResponseDto
    {
        public int PRId { get; set; }
        public int CapexRequestId { get; set; }
        public int DepartmentId { get; set; } // Required for manager filtering
        public string PRNumber { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
    }
}