using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class BudgetCategory : BaseEntity
    {
        [Key]
        public int BudgetCategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string CategoryCode { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<BudgetLine> BudgetLines { get; set; }
            = new List<BudgetLine>();
    }
}