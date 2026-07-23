using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissions([FromQuery] int? id)
        {
            var response = await _permissionService.GetAllPermissionsAsync(id);
            return response.Success ? Ok(response) : StatusCode(500, response);
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
            return response.Success ? Ok(response) : StatusCode(500, response);
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