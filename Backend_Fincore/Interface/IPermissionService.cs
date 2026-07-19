using Backend_Fincore.Models;
using Backend_Fincore.DTOs;
namespace Backend_Fincore.Interface
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> GetAllPermissionsAsync(int? id);
       
        Task<Permission> CreatePermissionAsync(PermissionDTO dto);
        Task<Permission?> UpdatePermissionAsync(int id, PermissionDTO dto);
        Task<bool> DeletePermissionAsync(int id);

    }
}
