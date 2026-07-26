using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using Backend_Fincore.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IRoleService
    {
        Task<List<RoleDTO>> GetAllRolesAsync(PaginationDTO pagination);
        Task<int> GetRoleCountAsync(PaginationDTO pagination);
        Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(int id);
        Task<ApiResponse<RoleDTO>> CreateRoleAsync(RoleDTO dto);
        Task<ApiResponse<RoleDTO>> UpdateRoleAsync(int id, RoleDTO dto);
        Task<ApiResponse<bool>> DeleteRoleAsync(int id);
    }
}