using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRolePermissionService
    {
        Task<(List<RolePermissionResponseDTO> Items, int TotalRecords)> GetAllAsync(PaginationDTO pagination);
        Task<RolePermissionResponseDTO> GetByIdAsync(int id);
        Task<List<RolePermissionResponseDTO>> GetByRoleIdAsync(int roleId);
        Task<RolePermissionResponseDTO> CreateAsync(RolePermissionDTO dto);
        Task<RolePermissionResponseDTO> UpdateAsync(int id, RolePermissionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}