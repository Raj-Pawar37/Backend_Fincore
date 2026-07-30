using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.DTOs.Role
{
    public class RoleDropdownDTO
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;
    }
}