namespace Backend_Fincore.Application.DTOs
{
    public class RevenueEntryCreateDto
    {
        public int CustomerId { get; set; }

        public string ProfitCenterName { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime RevenueDate { get; set; }

        public string? Status { get; set; }
    }
}