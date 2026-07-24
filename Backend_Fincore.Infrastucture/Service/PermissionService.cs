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

        public async Task<ApiResponse<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync(int? id)
        {
            try
            {
                var query = _db.Permission.AsQueryable();

                if (id.HasValue)
                {
                    query = query.Where(p => p.PermissionId == id);
                }

                var permissions = await query.ToListAsync();
                var dtos = _mapper.Map<IEnumerable<PermissionDTO>>(permissions);

                return new ApiResponse<IEnumerable<PermissionDTO>>
                {
                    Success = true,
                    Message = "Permissions fetched successfully",
                    Data = dtos,
                    TotalNumberRecord = dtos.Count()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<PermissionDTO>>
                {
                    Success = false,
                    Message = "Failed to fetch permissions",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PermissionDTO>> GetPermissionByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                return new ApiResponse<PermissionDTO>
                {
                    Success = false,
                    Message = "Failed to retrieve permission",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PermissionDTO>> CreatePermissionAsync(PermissionDTO dto)
        {
            try
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
            catch (Exception ex)
            {
                return new ApiResponse<PermissionDTO>
                {
                    Success = false,
                    Message = "Failed to create permission",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<PermissionDTO>> UpdatePermissionAsync(int id, PermissionDTO dto)
        {
            try
            {
                var permission = await _db.Permission.FindAsync(id);
                permission.ModifiedBy = current.UserId;
                permission.ModifiedAt = DateTime.UtcNow;

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
            catch (Exception ex)
            {
                return new ApiResponse<PermissionDTO>
                {
                    Success = false,
                    Message = "Failed to update permission",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> DeletePermissionAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to delete permission",
                    Data = false,
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }
    }
}