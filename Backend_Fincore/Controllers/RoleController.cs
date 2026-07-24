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
        public async Task<IActionResult> AllRoles([FromQuery] PaginationDTO pagination)
        {
            var res = await _roleService.GetAllRolesAsync(pagination);
            var totalRecords = await _roleService.GetRoleCountAsync(pagination);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<RoleDTO>>
            {
                Success = true,
                Message = "Roles fetched successfully.",
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
        public async Task<IActionResult> GetRole(int id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO dto)
        {
            var response = await _roleService.CreateRoleAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleDTO dto)
        {
            var response = await _roleService.UpdateRoleAsync(id, dto);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}