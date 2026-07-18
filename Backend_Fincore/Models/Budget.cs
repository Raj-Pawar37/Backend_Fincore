using Backend_Fincore.Models.Backend_Fincore.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Budget : BaseEntity
    {
        [Key]
        public int BudgetId { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public string FinancialYear { get; set; } = null!;

        public decimal TotalBudget { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }


        public Company Company { get; set; } = null!;

        public Department Department { get; set; } = null!;

        public User? ApprovedByUser { get; set; }

        public ICollection<BudgetLine> BudgetLines { get; set; }
            = new List<BudgetLine>();
    }
}