using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace Backend_Fincore.Controllers
{
    [Authorize]
    [Route("api/v1/role_permissions")]
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
        public async Task<IActionResult> GetAll()
        {
            var response = await _rolePermissionService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _rolePermissionService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("role/{roleId}")]
        public async Task<IActionResult> GetByRoleId(int roleId)
        {
            var response = await _rolePermissionService.GetByRoleIdAsync(roleId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolePermissionDTOs dto)
        {
            var response = await _rolePermissionService.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RolePermissionDTOs dto)
        {
            var response = await _rolePermissionService.UpdateAsync(id, dto);
            if (!response.Success)
            {
                return response.Error?.GetType().GetProperty("code")?.GetValue(response.Error)?.ToString() == "NOT_FOUND"
                    ? NotFound(response)
                    : BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _rolePermissionService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}