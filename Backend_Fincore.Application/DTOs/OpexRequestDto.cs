namespace Backend_Fincore.DTOs
{
    public class OpexRequestDto
    {

        public int OpexRequestId { get; set; }
        public int BudgetLineId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public int RequestedBy { get; set; }

        // Pending / Approved / Rejected
        public string Status { get; set; } = null!;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
