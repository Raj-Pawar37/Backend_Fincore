using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Backend_Fincore.Response; // Using your ApiResponse namespace

namespace Backend_Fincore.Interface
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleDTO>>> GetAllRolesAsync();
        Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(int id);
        Task<ApiResponse<RoleDTO>> CreateRoleAsync(RoleDTO dto);
        Task<ApiResponse<RoleDTO>> UpdateRoleAsync(int id, RoleDTO dto);
        Task<ApiResponse<bool>> DeleteRoleAsync(int id);
    }
}