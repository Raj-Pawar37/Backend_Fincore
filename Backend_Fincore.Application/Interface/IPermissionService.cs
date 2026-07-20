
using Backend_Fincore.DTOs;
namespace Backend_Fincore.Interface
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync(int? id);
       
        Task<PermissionDTO> CreatePermissionAsync(PermissionDTO dto);
        Task<PermissionDTO?> UpdatePermissionAsync(int id, PermissionDTO dto);
        Task<bool> DeletePermissionAsync(int id);

    }
}
