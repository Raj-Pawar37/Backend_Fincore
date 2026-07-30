using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.DTOs;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.Controllers
{
    [Authorize]
    [Route("api/v1/rolepermissions")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllRolePermissions([FromQuery] PaginationDTO pagination)
        {
            var (items, totalRecords) = await _rolePermissionService.GetAllAsync(pagination);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<RolePermissionResponseDTO>>
            {
                Success = true,
                Message = "Role permissions fetched successfully.",
                Data = items,
                TotalNumberRecord = totalRecords,
                Metadata = new
                {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = items.Count
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getRolePermissionById(int id)
        {
            var res = await _rolePermissionService.GetByIdAsync(id);
            return Ok(new ApiResponse<RolePermissionResponseDTO>
            {
                Success = true,
                Message = "Role permission retrieved successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> getRolePermissionsByRoleId(int roleId)
        {
            var res = await _rolePermissionService.GetByRoleIdAsync(roleId);
            return Ok(new ApiResponse<List<RolePermissionResponseDTO>>
            {
                Success = true,
                Message = "Role permissions for specified role fetched successfully.",
                Data = res,
                TotalNumberRecord = res.Count
            });
        }

        [HttpPost]
        public async Task<IActionResult> createRolePermission([FromBody] RolePermissionDTO dto)
        {
            var res = await _rolePermissionService.CreateAsync(dto);
            return Ok(new ApiResponse<RolePermissionResponseDTO>
            {
                Success = true,
                Message = "Role permission assigned successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateRolePermission(int id, [FromBody] RolePermissionDTO dto)
        {
            var res = await _rolePermissionService.UpdateAsync(id, dto);
            return Ok(new ApiResponse<RolePermissionResponseDTO>
            {
                Success = true,
                Message = "Role permission updated successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteRolePermission(int id)
        {
            await _rolePermissionService.DeleteAsync(id);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Role permission removed successfully.",
                Data = true,
                TotalNumberRecord = 1
            });
        }
    }
}