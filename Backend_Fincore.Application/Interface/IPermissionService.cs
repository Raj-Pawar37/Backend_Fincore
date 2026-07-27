using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Permission;
using Backend_Fincore.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IPermissionService
    {
        Task<(List<PermissionDTO> Items, int TotalRecords)> GetAllPermissionsAsync(PaginationDTO pagination);
        Task<PermissionDTO> GetPermissionByIdAsync(int id);
        Task<PermissionDTO> CreatePermissionAsync(PermissionDTO dto);
        Task<PermissionDTO> UpdatePermissionAsync(int id, PermissionDTO dto);
        Task<bool> DeletePermissionAsync(int id);
        Task<List<PermissionDropdownDTO>> GetPermissionDropdown(string? searchText);
    }
}