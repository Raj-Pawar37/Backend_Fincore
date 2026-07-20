using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class BudgetLine : BaseEntity
    {
        [Key]
        public int BudgetLineId { get; set; }

        public int BudgetId { get; set; }

        public string CostCenter { get; set; } = null!;

        public int BudgetCategoryId { get; set; }

        public decimal AllocatedAmount { get; set; }


        public Budget Budget { get; set; } = null!;

        public BudgetCategory BudgetCategory { get; set; } = null!;

        public ICollection<CapexRequest> CapexRequests { get; set; }
            = new List<CapexRequest>();

        public ICollection<OpexRequest> OpexRequests { get; set; }
            = new List<OpexRequest>();
    }
}