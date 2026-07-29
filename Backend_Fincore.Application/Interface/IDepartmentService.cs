using Backend_Fincore.Application.DTOs;
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
        Task<List<DepartmentReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalRecordDepartment();
        Task<DepartmentReadDTO> GetById(int id);

        Task<DepartmentReadDTO> AddDepartment(DepartmentWriteDTO dto);

        Task UpdateDepartment( int id, DepartmentUpdateDTO dto);

        Task DeleteDepartment(int id);


        Task<List<DepartmentDropdownDTO>>GetDepartmentDropdown(string? searchText);
    }
}
