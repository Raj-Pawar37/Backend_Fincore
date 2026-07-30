using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Role;
using Backend_Fincore.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRoleService
    {
        Task<(List<RoleDTO> Items, int TotalRecords)> GetAllRolesAsync(PaginationDTO pagination);
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task<RoleDTO> CreateRoleAsync(RoleDTO dto);
        Task<RoleDTO> UpdateRoleAsync(int id, RoleDTO dto);
        Task<bool> DeleteRoleAsync(int id);
        Task<List<RoleDropdownDTO>> GetRoleDropdown(string? searchText);
    }
}