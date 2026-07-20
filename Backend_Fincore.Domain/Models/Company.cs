using Backend_Fincore.Models.Backend_Fincore.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace Backend_Fincore.Models
{
    public class Company : BaseEntity
    {
        [Key]
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string CompanyCode { get; set; } = null!;

        public string? GSTNo { get; set; }

        public string PANNo { get; set; } = null!;

        public string ContactEmail { get; set; } = null!;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        // Navigation properties
        public Country Country { get; set; } = null!;

        public State State { get; set; } = null!;

        public City City { get; set; } = null!;

        public ICollection<Department> Departments { get; set; }
        = new List<Department>();

        public ICollection<Employee> Employees { get; set; }
            = new List<Employee>();

        public ICollection<Vendor> Vendors { get; set; }
            = new List<Vendor>();

        public ICollection<Customer> Customers { get; set; }
            = new List<Customer>();

        public ICollection<Budget> Budgets { get; set; }
            = new List<Budget>();

        public ICollection<JournalEntry> JournalEntries { get; set; }
            = new List<JournalEntry>();
    }
}
