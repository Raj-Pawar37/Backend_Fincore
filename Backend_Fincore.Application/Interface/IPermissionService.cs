using Backend_Fincore.DTOs;
using Backend_Fincore.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Interface
{
    public interface IPermissionService
    {
        Task<ApiResponse<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync(int? id);
        Task<ApiResponse<PermissionDTO>> GetPermissionByIdAsync(int id);
        Task<ApiResponse<PermissionDTO>> CreatePermissionAsync(PermissionDTO dto);
        Task<ApiResponse<PermissionDTO>> UpdatePermissionAsync(int id, PermissionDTO dto);
        Task<ApiResponse<bool>> DeletePermissionAsync(int id);
    }
}