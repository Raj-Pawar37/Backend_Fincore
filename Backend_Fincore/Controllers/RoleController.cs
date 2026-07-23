using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> AllRoles()
        {
            var response = await _roleService.GetAllRolesAsync();
            return response.Success ? Ok(response) : StatusCode(500, response);
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
            return response.Success ? Ok(response) : StatusCode(500, response);
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