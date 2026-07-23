using Backend_Fincore.DTOs;
using Backend_Fincore.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Interface
{
    public interface IRolePermissionService
    {
        Task<ApiResponse<IEnumerable<RolePermissionResponseDto>>> GetAllAsync();
        Task<ApiResponse<RolePermissionResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<RolePermissionResponseDto>>> GetByRoleIdAsync(int roleId);
        Task<ApiResponse<RolePermissionResponseDto>> CreateAsync(RolePermissionDTOs dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}