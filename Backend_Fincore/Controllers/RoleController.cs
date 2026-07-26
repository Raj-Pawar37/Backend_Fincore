using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Role;
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
    [Route("api/v1/roles")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllRoles([FromQuery] PaginationDTO pagination)
        {
            var (items, totalRecords) = await _roleService.GetAllRolesAsync(pagination);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<RoleDTO>>
            {
                Success = true,
                Message = "Roles fetched successfully.",
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
        public async Task<IActionResult> getRoleById(int id)
        {
            var res = await _roleService.GetRoleByIdAsync(id);
            return Ok(new ApiResponse<RoleDTO>
            {
                Success = true,
                Message = "Role retrieved successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> getRoleDropdown([FromQuery] string? searchText)
        {
            var list = await _roleService.GetRoleDropdown(searchText);
            return Ok(new ApiResponse<List<RoleDropdownDTO>>
            {
                Success = true,
                Message = "Role dropdown data fetched successfully.",
                Data = list,
                TotalNumberRecord = list.Count
            });
        }

        [HttpPost]
        public async Task<IActionResult> createRole([FromBody] RoleDTO dto)
        {
            var res = await _roleService.CreateRoleAsync(dto);
            return Ok(new ApiResponse<RoleDTO>
            {
                Success = true,
                Message = "Role created successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateRole(int id, [FromBody] RoleDTO dto)
        {
            var res = await _roleService.UpdateRoleAsync(id, dto);
            return Ok(new ApiResponse<RoleDTO>
            {
                Success = true,
                Message = "Role updated successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Role deleted successfully.",
                Data = true,
                TotalNumberRecord = 1
            });
        }
    }
}