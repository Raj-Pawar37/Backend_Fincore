namespace Backend_Fincore.Application.DTOs.PurchaseRequisition
{
    public class PRDropdownResponseDto
    {
        public int PRId { get; set; }
        public string PRNumber { get; set; } = null!;
        public string Title { get; set; } = null!;
    }
}