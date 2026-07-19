using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public RoleService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _db.Role.ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _db.Role.FindAsync(id);
        }

        public async Task<Role> CreateRoleAsync(RoleDTO dto)
        { 
            var role = _mapper.Map<Role>(dto);
            _db.Role.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task<Role?> UpdateRoleAsync(int id, RoleDTO dto)
        {
            var role = await _db.Role.FindAsync(id);
            if (role == null)
            {
                return null;
            }

            _mapper.Map(dto, role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _db.Role.FindAsync(id);
            if (role == null)
            {
                return false;
            }

            _db.Role.Remove(role);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
