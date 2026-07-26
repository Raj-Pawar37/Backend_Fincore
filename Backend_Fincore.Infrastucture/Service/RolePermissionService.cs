using AutoMapper;
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
    public class RolePermissionService : IRolePermissionService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService current;

        public RolePermissionService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            _db = db;
            _mapper = mapper;
            this.current = current;
        }

        public async Task<ApiResponse<IEnumerable<RolePermissionResponseDto>>> GetAllAsync()
        {
            var rolePermissions = await _db.RolePermission
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<RolePermissionResponseDto>>(rolePermissions);

            return new ApiResponse<IEnumerable<RolePermissionResponseDto>>
            {
                Success = true,
                Message = "Role permissions fetched successfully",
                Data = dtos,
                TotalNumberRecord = dtos.Count()
            };
        }

        public async Task<ApiResponse<RolePermissionResponseDto>> GetByIdAsync(int id)
        {
            var rolePermission = await _db.RolePermission
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RolePermissionId == id);

            if (rolePermission == null)
            {
                return new ApiResponse<RolePermissionResponseDto>
                {
                    Success = false,
                    Message = "Role permission mapping not found",
                    Error = new { code = "NOT_FOUND", details = $"RolePermission with ID {id} was not found." }
                };
            }

            var dto = _mapper.Map<RolePermissionResponseDto>(rolePermission);
            return new ApiResponse<RolePermissionResponseDto>
            {
                Success = true,
                Message = "Role permission fetched successfully",
                Data = dto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<IEnumerable<RolePermissionResponseDto>>> GetByRoleIdAsync(int roleId)
        {
            var rolePermissions = await _db.RolePermission
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<RolePermissionResponseDto>>(rolePermissions);

            return new ApiResponse<IEnumerable<RolePermissionResponseDto>>
            {
                Success = true,
                Message = "Role permissions for role fetched successfully",
                Data = dtos,
                TotalNumberRecord = dtos.Count()
            };
        }

        public async Task<ApiResponse<RolePermissionResponseDto>> CreateAsync(RolePermissionDTOs dto)
        {
            var roleExists = await _db.Role.AnyAsync(r => r.RoleId == dto.RoleId);
            var permissionExists = await _db.Permission.AnyAsync(p => p.PermissionId == dto.PermissionId);

            if (!roleExists || !permissionExists)
            {
                return new ApiResponse<RolePermissionResponseDto>
                {
                    Success = false,
                    Message = "Invalid Role or Permission ID",
                    Error = new { code = "BAD_REQUEST", details = "The specified Role or Permission does not exist." }
                };
            }

            var rolePermission = _mapper.Map<RolePermission>(dto);
            rolePermission.CreatedBy = current.UserId;
            rolePermission.CreatedAt = DateTime.UtcNow;

            _db.RolePermission.Add(rolePermission);
            await _db.SaveChangesAsync();

            await _db.Entry(rolePermission).Reference(rp => rp.Role).LoadAsync();
            await _db.Entry(rolePermission).Reference(rp => rp.Permission).LoadAsync();

            var createdDto = _mapper.Map<RolePermissionResponseDto>(rolePermission);
            return new ApiResponse<RolePermissionResponseDto>
            {
                Success = true,
                Message = "Role permission assigned successfully",
                Data = createdDto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<RolePermissionResponseDto>> UpdateAsync(int id, RolePermissionDTOs dto)
        {
            var rolePermission = await _db.RolePermission.FindAsync(id);

            if (rolePermission == null)
            {
                return new ApiResponse<RolePermissionResponseDto>
                {
                    Success = false,
                    Message = "Role permission mapping not found",
                    Error = new { code = "NOT_FOUND", details = $"RolePermission with ID {id} was not found." }
                };
            }

            var roleExists = await _db.Role.AnyAsync(r => r.RoleId == dto.RoleId);
            var permissionExists = await _db.Permission.AnyAsync(p => p.PermissionId == dto.PermissionId);

            if (!roleExists || !permissionExists)
            {
                return new ApiResponse<RolePermissionResponseDto>
                {
                    Success = false,
                    Message = "Invalid Role or Permission ID",
                    Error = new { code = "BAD_REQUEST", details = "The specified Role or Permission does not exist." }
                };
            }

            _mapper.Map(dto, rolePermission);
            rolePermission.ModifiedBy = current.UserId;
            rolePermission.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _db.Entry(rolePermission).Reference(rp => rp.Role).LoadAsync();
            await _db.Entry(rolePermission).Reference(rp => rp.Permission).LoadAsync();

            var updatedDto = _mapper.Map<RolePermissionResponseDto>(rolePermission);
            return new ApiResponse<RolePermissionResponseDto>
            {
                Success = true,
                Message = "Role permission updated successfully",
                Data = updatedDto,
                TotalNumberRecord = 1
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var rolePermission = await _db.RolePermission.FindAsync(id);
            if (rolePermission == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Role permission mapping not found",
                    Data = false,
                    Error = new { code = "NOT_FOUND", details = $"RolePermission with ID {id} was not found." }
                };
            }

            _db.RolePermission.Remove(rolePermission);
            await _db.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Role permission removed successfully",
                Data = true,
                TotalNumberRecord = 1
            };
        }

        public Task<ApiResponse<bool>> DeleteRolePermissionAsync(int id)
        {
            return DeleteAsync(id);
        }
    }
}