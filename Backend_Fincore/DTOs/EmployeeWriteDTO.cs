using System.ComponentModel.DataAnnotations;

namespace Backend_Fincore.DTOs
{
    public class EmployeeWriteDTO
    {
        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }
        [Required]
        public string EmployeeCode { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }
        [Required]
        public string Designation { get; set; } = null!;

        public DateTime JoiningDate { get; set; }

        public int? ReportingManagerId { get; set; }
    }
}
