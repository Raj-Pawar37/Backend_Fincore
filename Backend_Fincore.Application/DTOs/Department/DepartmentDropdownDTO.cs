using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Department
{
    public class DepartmentDropdownDTO
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;
    }
}
