using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Department
{
    public class DepartmentWriteDTO
    {
        [Required]
        public int CompanyId { get; set; }

        [Required]
        public string DepartmentName { get; set; } = null!;

        [Required]
        public string DepartmentCode { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
