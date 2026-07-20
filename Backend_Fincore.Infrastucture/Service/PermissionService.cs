using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_Fincore.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PermissionService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        
       public async Task<Permission> CreatePermissionAsync(PermissionDTO dto)
        {
            var permission = _mapper.Map<Permission>(dto);
            _db.Permission.Add(permission);
            await _db.SaveChangesAsync();
            return permission;

        }

       public async Task<bool> DeletePermissionAsync(int id)
        {
           var permission = await _db.Permission.FindAsync(id);
            if(permission != null)
            {
                _db.Permission.Remove(permission);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;

        }

        public async Task<IEnumerable<Permission>> GetAllPermissionsAsync(int? id)
        {

            if (id.HasValue)
            {
                return await _db.Permission.Where(p => p.PermissionId == id).ToListAsync();
            }

            return await _db.Permission.ToListAsync();
        }

        public async Task<Permission> UpdatePermissionAsync(int id, PermissionDTO dto)
        {
            var permission = await  _db.Permission.FindAsync(id);
            if(permission != null)
            {
                _mapper.Map<PermissionDTO, Permission>(dto, permission);
                await _db.SaveChangesAsync();
                return permission;
            }
            else
            {
                return null;
            }
        }

        Task<PermissionDTO> IPermissionService.CreatePermissionAsync(PermissionDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<PermissionDTO>> IPermissionService.GetAllPermissionsAsync(int? id)
        {
            throw new NotImplementedException();
        }

        Task<PermissionDTO?> IPermissionService.UpdatePermissionAsync(int id, PermissionDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
