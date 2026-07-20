using Backend_Fincore.Models.Backend_Fincore.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.Models
{
    public class Employee : BaseEntity
    {
        [Key]
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public string EmployeeCode { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string Designation { get; set; } = null!;

        public DateTime JoiningDate { get; set; }

        public int? ReportingManagerId { get; set; }

        // Navigation properties
        public Company Company { get; set; } = null!;

        public Department Department { get; set; }

        public Employee? ReportingManager { get; set; }

        public ICollection<Employee> ReportingEmployees { get; set; }
            = new List<Employee>();
    }
}
