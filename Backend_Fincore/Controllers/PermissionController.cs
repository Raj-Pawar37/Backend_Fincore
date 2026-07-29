using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Permission;
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
    [Route("api/v1/permissions")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllPermissions([FromQuery] PaginationDTO pagination)
        {
            var (items, totalRecords) = await _permissionService.GetAllPermissionsAsync(pagination);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<PermissionDTO>>
            {
                Success = true,
                Message = "Permissions fetched successfully.",
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
        public async Task<IActionResult> getPermissionById(int id)
        {
            var res = await _permissionService.GetPermissionByIdAsync(id);
            return Ok(new ApiResponse<PermissionDTO>
            {
                Success = true,
                Message = "Permission retrieved successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> getPermissionDropdown([FromQuery] string? searchText)
        {
            var list = await _permissionService.GetPermissionDropdown(searchText);
            return Ok(new ApiResponse<List<PermissionDropdownDTO>>
            {
                Success = true,
                Message = "Permission dropdown data fetched successfully.",
                Data = list,
                TotalNumberRecord = list.Count
            });
        }

        [HttpPost]
        public async Task<IActionResult> createPermission([FromBody] PermissionDTO dto)
        {
            var res = await _permissionService.CreatePermissionAsync(dto);
            return Ok(new ApiResponse<PermissionDTO>
            {
                Success = true,
                Message = "Permission created successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updatePermission(int id, [FromBody] PermissionDTO dto)
        {
            var res = await _permissionService.UpdatePermissionAsync(id, dto);
            return Ok(new ApiResponse<PermissionDTO>
            {
                Success = true,
                Message = "Permission updated successfully.",
                Data = res,
                TotalNumberRecord = 1
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deletePermission(int id)
        {
            await _permissionService.DeletePermissionAsync(id);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Permission deleted successfully.",
                Data = true,
                TotalNumberRecord = 1
            });
        }
    }
}