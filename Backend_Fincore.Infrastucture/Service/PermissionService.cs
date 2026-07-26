using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Fincore.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService current;

        public PermissionService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            _db = db;
            _mapper = mapper;
            this.current = current;
        }

        public async Task<int> GetPermissionCountAsync(PaginationDTO pagination)
        {
            var query = _db.Permission.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(p => p.PermissionName.Contains(pagination.Search));
            }

            return await query.CountAsync();
        }

        public async Task<List<PermissionDTO>> GetAllPermissionsAsync(PaginationDTO pagination)
        {
            var query = _db.Permission.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(p => p.PermissionName.Contains(pagination.Search));
            }

            var data = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return _mapper.Map<List<PermissionDTO>>(data);
        }

        public async Task<ApiResponse<PermissionDTO>> GetPermissionByIdAsync(int id)
        {
            var permission = await _db.Permission.FindAsync(id);
            if (permission == null)
            {
                return new ApiResponse<PermissionDTO>
                {
                    Success = false,
                    Message = "Permission not found",
                    Error = new { code = "NOT_FOUND", details = $"Permission with ID {id} was not found." }
                };
            }

            var dto = _mapper.Map<PermissionDTO>(permission);
            return new ApiResponse<PermissionDTO>
            {
                Success = true,
                Message = "Permission found successfully",
                Data = dto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<PermissionDTO>> CreatePermissionAsync(PermissionDTO dto)
        {
            var permission = _mapper.Map<Permission>(dto);
            permission.CreatedBy = current.UserId;
            permission.CreatedAt = DateTime.UtcNow;

            _db.Permission.Add(permission);
            await _db.SaveChangesAsync();

            var createdDto = _mapper.Map<PermissionDTO>(permission);
            return new ApiResponse<PermissionDTO>
            {
                Success = true,
                Message = "Permission created successfully",
                Data = createdDto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<PermissionDTO>> UpdatePermissionAsync(int id, PermissionDTO dto)
        {
            var permission = await _db.Permission.FindAsync(id);

            if (permission == null)
            {
                return new ApiResponse<PermissionDTO>
                {
                    Success = false,
                    Message = "Permission not found",
                    Error = new { code = "NOT_FOUND", details = $"Permission with ID {id} was not found." }
                };
            }

            _mapper.Map(dto, permission);
            permission.ModifiedBy = current.UserId;
            permission.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var updatedDto = _mapper.Map<PermissionDTO>(permission);
            return new ApiResponse<PermissionDTO>
            {
                Success = true,
                Message = "Permission updated successfully",
                Data = updatedDto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<bool>> DeletePermissionAsync(int id)
        {
            var permission = await _db.Permission.FindAsync(id);
            if (permission == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Permission not found",
                    Data = false,
                    Error = new { code = "NOT_FOUND", details = $"Permission with ID {id} was not found." }
                };
            }

            _db.Permission.Remove(permission);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Permission deleted successfully",
                Data = true,
                TotalNumberRecord = 1
            };
        }
    }
}