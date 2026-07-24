using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
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
        public async Task<IActionResult> GetAllPermissions([FromQuery] PaginationDTO pagination)
        {
            var res = await _permissionService.GetAllPermissionsAsync(pagination);
            var totalRecords = await _permissionService.GetPermissionCountAsync(pagination);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<PermissionDTO>>
            {
                Success = true,
                Message = "Permissions fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = totalRecords,
                Metadata = new
                {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = res.Count
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var response = await _permissionService.GetPermissionByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDTO dto)
        {
            var response = await _permissionService.CreatePermissionAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionDTO dto)
        {
            var response = await _permissionService.UpdatePermissionAsync(id, dto);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var response = await _permissionService.DeletePermissionAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}