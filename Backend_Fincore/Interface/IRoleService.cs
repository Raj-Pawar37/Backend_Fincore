using Backend_Fincore.DTOs;
using Backend_Fincore.Models;

namespace Backend_Fincore.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<Role> CreateRoleAsync(RoleDTO dto);
        Task<Role?> UpdateRoleAsync(int id, RoleDTO dto);
        Task<bool> DeleteRoleAsync(int id);
    }
}
