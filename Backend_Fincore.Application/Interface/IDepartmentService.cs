using Backend_Fincore.Application.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IDepartmentService
    {
        Task<List<DepartmentReadDTO>> GetAll();

        Task<DepartmentReadDTO> GetById(int id);

        Task<DepartmentReadDTO> AddDepartment(DepartmentWriteDTO dto);

        Task UpdateDepartment( int id,DepartmentWriteDTO dto);

        Task DeleteDepartment(int id);
    }
}
