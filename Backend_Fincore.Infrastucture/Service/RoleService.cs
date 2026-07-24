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
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService current;

        public RoleService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            _db = db;
            _mapper = mapper;
            this.current = current;
        }

        public async Task<ApiResponse<IEnumerable<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _db.Role.ToListAsync();
                var dtos = _mapper.Map<IEnumerable<RoleDTO>>(roles);

                return new ApiResponse<IEnumerable<RoleDTO>>
                {
                    Success = true,
                    Message = "Roles fetched successfully",
                    Data = dtos,
                    TotalNumberRecord = dtos.Count()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<RoleDTO>>
                {
                    Success = false,
                    Message = "Failed to fetch roles",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(int id)
        {
            try
            {
                var role = await _db.Role.FindAsync(id);
                if (role == null)
                {
                    return new ApiResponse<RoleDTO>
                    {
                        Success = false,
                        Message = "Role not found",
                        Error = new { code = "NOT_FOUND", details = $"Role with ID {id} was not found." }
                    };
                }

                var dto = _mapper.Map<RoleDTO>(role);
                return new ApiResponse<RoleDTO>
                {
                    Success = true,
                    Message = "Role found successfully",
                    Data = dto,
                    TotalNumberRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleDTO>
                {
                    Success = false,
                    Message = "Failed to retrieve role",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<RoleDTO>> CreateRoleAsync(RoleDTO dto)
        {
            try
            {
                var role = _mapper.Map<Role>(dto);
                role.ModifiedBy = current.UserId;
                role.ModifiedAt = DateTime.Now;
                _db.Role.Add(role);

                await _db.SaveChangesAsync();

                var createdDto = _mapper.Map<RoleDTO>(role);
                return new ApiResponse<RoleDTO>
                {
                    Success = true,
                    Message = "Role created successfully",
                    Data = createdDto,
                    TotalNumberRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleDTO>
                {
                    Success = false,
                    Message = "Failed to create role",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<RoleDTO>> UpdateRoleAsync(int id, RoleDTO dto)
        {
            try
            {
                var role = await _db.Role.FindAsync(id);
                if (role == null)
                {
                    return new ApiResponse<RoleDTO>
                    {
                        Success = false,
                        Message = "Role not found",
                        Error = new { code = "NOT_FOUND", details = $"Role with ID {id} was not found." }
                    };
                }

                _mapper.Map(dto, role);
                await _db.SaveChangesAsync();

                var updatedDto = _mapper.Map<RoleDTO>(role);
                return new ApiResponse<RoleDTO>
                {
                    Success = true,
                    Message = "Role updated successfully",
                    Data = updatedDto,
                    TotalNumberRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoleDTO>
                {
                    Success = false,
                    Message = "Failed to update role",
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(int id)
        {
            try
            {
                var role = await _db.Role.FindAsync(id);
                if (role == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Role not found",
                        Data = false,
                        Error = new { code = "NOT_FOUND", details = $"Role with ID {id} was not found." }
                    };
                }

                _db.Role.Remove(role);
                await _db.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Role deleted successfully",
                    Data = true,
                    TotalNumberRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to delete role",
                    Data = false,
                    Error = new { code = "INTERNAL_ERROR", details = ex.Message }
                };
            }
        }
    }
}