using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs; // <--- Fixes CS0246 and CS0535
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _current;
        private readonly IMemoryCache _cache;

        private const string CacheKeyList = "Cache_RolePermissions_List_";

        public RolePermissionService(
            AppDbContext db,
            IMapper mapper,
            ICurrentUserService current,
            IMemoryCache cache)
        {
            _db = db;
            _mapper = mapper;
            _current = current;
            _cache = cache;
        }

        public async Task<(List<RolePermissionResponseDTO> Items, int TotalRecords)> GetAllAsync(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (_cache.TryGetValue(cacheKey, out (List<RolePermissionResponseDTO> Items, int TotalRecords) cachedResult))
            {
                return cachedResult;
            }

            var query = _db.RolePermission
                .AsNoTracking()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Where(rp => rp.IsActive == 1 && rp.Role.IsActive == 1 && rp.Permission.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                var search = pagination.Search.Trim().ToLower();
                // Fixes CS1503: Simple, safe string contains in EF Core LINQ query
                query = query.Where(rp => rp.Role.RoleName.ToLower().Contains(search) ||
                                          rp.Permission.PermissionName.ToLower().Contains(search));
            }

            int totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(rp => rp.RolePermissionId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var mappedData = _mapper.Map<List<RolePermissionResponseDTO>>(data);
            var result = (mappedData, totalRecords);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<RolePermissionResponseDTO> GetByIdAsync(int id)
        {
            var rolePermission = await _db.RolePermission
                .AsNoTracking()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RolePermissionId == id && rp.IsActive == 1);

            if (rolePermission == null)
                throw new KeyNotFoundException($"RolePermission mapping with ID {id} was not found or is inactive.");

            return _mapper.Map<RolePermissionResponseDTO>(rolePermission);
        }

        public async Task<List<RolePermissionResponseDTO>> GetByRoleIdAsync(int roleId)
        {
            var roleExists = await _db.Role.AnyAsync(r => r.RoleId == roleId && r.IsActive == 1);
            if (!roleExists)
                throw new KeyNotFoundException($"Role with ID {roleId} was not found or is inactive.");

            var rolePermissions = await _db.RolePermission
                .AsNoTracking()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId && rp.IsActive == 1)
                .ToListAsync();

            return _mapper.Map<List<RolePermissionResponseDTO>>(rolePermissions);
        }

        public async Task<RolePermissionResponseDTO> CreateAsync(RolePermissionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "RolePermission payload cannot be null.");

            var roleExists = await _db.Role.AnyAsync(r => r.RoleId == dto.RoleId && r.IsActive == 1);
            if (!roleExists)
                throw new KeyNotFoundException($"Role with ID {dto.RoleId} does not exist or is inactive.");

            var permissionExists = await _db.Permission.AnyAsync(p => p.PermissionId == dto.PermissionId && p.IsActive == 1);
            if (!permissionExists)
                throw new KeyNotFoundException($"Permission with ID {dto.PermissionId} does not exist or is inactive.");

            var rolePermission = _mapper.Map<RolePermission>(dto);
            rolePermission.IsActive = 1;
            rolePermission.CreatedBy = _current.UserId;
            rolePermission.CreatedAt = DateTime.UtcNow;

            _db.RolePermission.Add(rolePermission);
            await _db.SaveChangesAsync();

            await _db.Entry(rolePermission).Reference(rp => rp.Role).LoadAsync();
            await _db.Entry(rolePermission).Reference(rp => rp.Permission).LoadAsync();

            ClearCache();

            return _mapper.Map<RolePermissionResponseDTO>(rolePermission);
        }

        public async Task<RolePermissionResponseDTO> UpdateAsync(int id, RolePermissionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "RolePermission payload cannot be null.");

            var rolePermission = await _db.RolePermission.FindAsync(id);

            if (rolePermission == null)
                throw new KeyNotFoundException($"RolePermission mapping with ID {id} was not found.");

            var roleExists = await _db.Role.AnyAsync(r => r.RoleId == dto.RoleId && r.IsActive == 1);
            if (!roleExists)
                throw new KeyNotFoundException($"Role with ID {dto.RoleId} does not exist or is inactive.");

            var permissionExists = await _db.Permission.AnyAsync(p => p.PermissionId == dto.PermissionId && p.IsActive == 1);
            if (!permissionExists)
                throw new KeyNotFoundException($"Permission with ID {dto.PermissionId} does not exist or is inactive.");

            if (rolePermission.IsActive == 0 && dto.IsActive)
            {
                rolePermission.IsActive = 1;
            }
            else if (rolePermission.IsActive == 1 && !dto.IsActive)
            {
                throw new InvalidOperationException("To deactivate a role permission mapping, perform a DELETE request.");
            }

            _mapper.Map(dto, rolePermission);
            rolePermission.RolePermissionId = id;
            rolePermission.ModifiedBy = _current.UserId;
            rolePermission.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _db.Entry(rolePermission).Reference(rp => rp.Role).LoadAsync();
            await _db.Entry(rolePermission).Reference(rp => rp.Permission).LoadAsync();

            ClearCache();

            return _mapper.Map<RolePermissionResponseDTO>(rolePermission);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rolePermission = await _db.RolePermission.FindAsync(id);

            if (rolePermission == null || rolePermission.IsActive == 0)
                throw new KeyNotFoundException($"RolePermission mapping with ID {id} was not found or is already inactive.");

            rolePermission.IsActive = 0;
            rolePermission.ModifiedBy = _current.UserId;
            rolePermission.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            ClearCache();

            return true;
        }

        private void ClearCache()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}