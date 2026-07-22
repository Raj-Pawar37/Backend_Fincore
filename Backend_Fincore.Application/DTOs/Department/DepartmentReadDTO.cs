using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Department
{
    public class DepartmentReadDTO
    {
        public int DepartmentId { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string DepartmentName { get; set; } = null!;

        public string DepartmentCode { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
