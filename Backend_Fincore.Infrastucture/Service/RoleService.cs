using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.EntityFrameworkCore;
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
        private readonly ICurrentUserService current;

        public RoleService(AppDbContext db, IMapper mapper, ICurrentUserService current)
        {
            _db = db;
            _mapper = mapper;
            this.current = current;
        }

        public async Task<int> GetRoleCountAsync(PaginationDTO pagination)
        {
            var query = _db.Role.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.RoleName.Contains(pagination.Search));
            }

            return await query.CountAsync();
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync(PaginationDTO pagination)
        {
            var query = _db.Role.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                query = query.Where(x => x.RoleName.Contains(pagination.Search));
            }

            var data = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return _mapper.Map<List<RoleDTO>>(data);
        }

        public async Task<ApiResponse<RoleDTO>> GetRoleByIdAsync(int id)
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

        public async Task<ApiResponse<RoleDTO>> CreateRoleAsync(RoleDTO dto)
        {
            var role = _mapper.Map<Role>(dto);
            role.CreatedBy = current.UserId;
            role.CreatedAt = DateTime.UtcNow;

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

        public async Task<ApiResponse<RoleDTO>> UpdateRoleAsync(int id, RoleDTO dto)
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
            role.ModifiedBy = current.UserId;
            role.ModifiedAt = DateTime.UtcNow;

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

        public async Task<ApiResponse<bool>> DeleteRoleAsync(int id)
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
    }
}