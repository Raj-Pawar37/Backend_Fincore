namespace Backend_Fincore.Models
{
    namespace Backend_Fincore.Models
    {
        public class Department : BaseEntity
        {
            public int DepartmentId { get; set; }

            public int CompanyId { get; set; }

            public string DepartmentName { get; set; } = null!;

            public string DepartmentCode { get; set; } = null!;

            public string? Description { get; set; }


            // Navigation properties
            public Company Company { get; set; } = null!;

            public ICollection<Employee> Employees { get; set; }
                = new List<Employee>();

            public ICollection<Budget> Budgets { get; set; }
    = new List<Budget>();
        }
    }
}
