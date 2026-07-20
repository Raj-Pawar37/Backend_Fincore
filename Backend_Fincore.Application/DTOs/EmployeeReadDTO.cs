namespace Backend_Fincore.DTOs
{
    public class EmployeeReadDTO
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;

        public string EmployeeCode { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string Designation { get; set; } = null!;

        public DateTime JoiningDate { get; set; }

        public int? ReportingManagerId { get; set; }

        public string? ReportingManagerName { get; set; }
    }
}