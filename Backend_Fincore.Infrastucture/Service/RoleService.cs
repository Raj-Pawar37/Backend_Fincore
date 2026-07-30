using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Role;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _current;
        private readonly IMemoryCache _cache;

        private const string CacheKeyList = "Cache_Roles_List_";
        private const string CacheKeyDropdown = "Cache_Role_Dropdown_";

        public RoleService(
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

        public async Task<(List<RoleDTO> Items, int TotalRecords)> GetAllRolesAsync(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (_cache.TryGetValue(cacheKey, out (List<RoleDTO> Items, int TotalRecords) cachedResult))
            {
                return cachedResult;
            }

            var query = _db.Role.AsNoTracking().Where(x => x.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                var search = pagination.Search.Trim().ToLower();
                query = query.Where(x => x.RoleName.ToLower().Contains(search) || x.RoleCode.ToLower().Contains(search));
            }

            int totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.RoleId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var mappedData = _mapper.Map<List<RoleDTO>>(data);
            var result = (mappedData, totalRecords);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            var role = await _db.Role.AsNoTracking().FirstOrDefaultAsync(x => x.RoleId == id && x.IsActive == 1);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} was not found or is inactive.");

            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<List<RoleDropdownDTO>> GetRoleDropdown(string? searchText)
        {
            string cacheKey = $"{CacheKeyDropdown}{searchText ?? "All"}";

            if (_cache.TryGetValue(cacheKey, out List<RoleDropdownDTO>? cachedDropdown) && cachedDropdown != null)
            {
                return cachedDropdown;
            }

            var search = _db.Role.AsNoTracking().Where(x => x.IsActive == 1);

            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x => x.RoleName.Contains(searchText));
            }

            var data = await search
                            .OrderBy(x => x.RoleName)
                            .Take(20)
                            .Select(x => new RoleDropdownDTO
                            {
                                RoleId = x.RoleId,
                                RoleName = x.RoleName
                            })
                            .ToListAsync();

            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(15));

            return data;
        }

        public async Task<RoleDTO> CreateRoleAsync(RoleDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Role payload cannot be null.");

            var role = _mapper.Map<Role>(dto);
            role.IsActive = 1;
            role.CreatedBy = _current.UserId;
            role.CreatedAt = DateTime.UtcNow;

            _db.Role.Add(role);
            await _db.SaveChangesAsync();

            ClearRoleCache();

            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<RoleDTO> UpdateRoleAsync(int id, RoleDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Role payload cannot be null.");

            var role = await _db.Role.FindAsync(id);

            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} was not found.");

            // Transition check (0 -> 1 allowed, 1 -> 0 restricted)
            if (role.IsActive == 0 && dto.IsActive)
            {
                role.IsActive = 1;
            }
            else if (role.IsActive == 1 && !dto.IsActive)
            {
                throw new InvalidOperationException("To deactivate a role, perform a DELETE request.");
            }

            _mapper.Map(dto, role);
            role.RoleId = id;
            role.ModifiedBy = _current.UserId;
            role.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            ClearRoleCache();

            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await _db.Role.FindAsync(id);

            if (role == null || role.IsActive == 0)
                throw new KeyNotFoundException($"Role with ID {id} was not found or is already inactive.");

            role.IsActive = 0;
            role.ModifiedBy = _current.UserId;
            role.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            ClearRoleCache();

            return true;
        }

        private void ClearRoleCache()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}