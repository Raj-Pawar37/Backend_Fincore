using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Permission;
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
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _current;
        private readonly IMemoryCache _cache;

        private const string CacheKeyList = "Cache_Permissions_List_";
        private const string CacheKeyDropdown = "Cache_Permission_Dropdown_";

        public PermissionService(
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

        public async Task<(List<PermissionDTO> Items, int TotalRecords)> GetAllPermissionsAsync(PaginationDTO pagination)
        {
            string cacheKey = $"{CacheKeyList}Page_{pagination.PageNumber}_Size_{pagination.PageSize}_Search_{pagination.Search ?? "None"}";

            if (_cache.TryGetValue(cacheKey, out (List<PermissionDTO> Items, int TotalRecords) cachedResult))
            {
                return cachedResult;
            }

            var query = _db.Permission.AsNoTracking().Where(x => x.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                var search = pagination.Search.Trim().ToLower();
                query = query.Where(x => x.PermissionName.ToLower().Contains(search) || x.ModuleName.ToLower().Contains(search));
            }

            int totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.PermissionId)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var mappedData = _mapper.Map<List<PermissionDTO>>(data);
            var result = (mappedData, totalRecords);

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            var permission = await _db.Permission.AsNoTracking().FirstOrDefaultAsync(x => x.PermissionId == id && x.IsActive == 1);

            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {id} was not found or is inactive.");

            return _mapper.Map<PermissionDTO>(permission);
        }

        public async Task<List<PermissionDropdownDTO>> GetPermissionDropdown(string? searchText)
        {
            string cacheKey = $"{CacheKeyDropdown}{searchText ?? "All"}";

            if (_cache.TryGetValue(cacheKey, out List<PermissionDropdownDTO>? cachedDropdown) && cachedDropdown != null)
            {
                return cachedDropdown;
            }

            var search = _db.Permission.AsNoTracking().Where(x => x.IsActive == 1);

            if (!string.IsNullOrEmpty(searchText))
            {
                search = search.Where(x => x.PermissionName.Contains(searchText));
            }

            var data = await search
                            .OrderBy(x => x.PermissionName)
                            .Take(20)
                            .Select(x => new PermissionDropdownDTO
                            {
                                PermissionId = x.PermissionId,
                                PermissionName = x.PermissionName
                            })
                            .ToListAsync();

            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(15));

            return data;
        }

        public async Task<PermissionDTO> CreatePermissionAsync(PermissionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Permission payload cannot be null.");

            var permission = _mapper.Map<Permission>(dto);
            permission.IsActive = 1;
            permission.CreatedBy = _current.UserId;
            permission.CreatedAt = DateTime.UtcNow;

            _db.Permission.Add(permission);
            await _db.SaveChangesAsync();

            ClearPermissionCache();

            return _mapper.Map<PermissionDTO>(permission);
        }

        public async Task<PermissionDTO> UpdatePermissionAsync(int id, PermissionDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Permission payload cannot be null.");

            var permission = await _db.Permission.FindAsync(id);

            if (permission == null)
                throw new KeyNotFoundException($"Permission with ID {id} was not found.");

            if (permission.IsActive == 0 && dto.IsActive)
            {
                permission.IsActive = 1;
            }
            else if (permission.IsActive == 1 && !dto.IsActive)
            {
                throw new InvalidOperationException("To deactivate a permission, perform a DELETE request.");
            }

            _mapper.Map(dto, permission);
            permission.PermissionId = id;
            permission.ModifiedBy = _current.UserId;
            permission.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            ClearPermissionCache();

            return _mapper.Map<PermissionDTO>(permission);
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            var permission = await _db.Permission.FindAsync(id);

            if (permission == null || permission.IsActive == 0)
                throw new KeyNotFoundException($"Permission with ID {id} was not found or is already inactive.");

            permission.IsActive = 0;
            permission.ModifiedBy = _current.UserId;
            permission.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            ClearPermissionCache();

            return true;
        }

        private void ClearPermissionCache()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }
    }
}