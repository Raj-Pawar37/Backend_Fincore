using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Application.DTOs.OpexRequest
{
    public class OpexRequestWriteDTO
    {
        [Required]
        public int BudgetLineId { get; set; }

        [Required]

        public string Title { get; set; } = null!;

        [Required]
  
        public decimal Amount { get; set; }

        [Required]
        public int RequestedBy { get; set; }
    }
}

