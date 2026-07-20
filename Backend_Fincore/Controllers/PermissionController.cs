using AutoMapper;
using Backend_Fincore.Common;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Backend_Fincore.Controllers
{
    public class PermissionController : ControllerBase
    {


        private readonly IMapper mapper;
        private readonly IPermissionService services;

        public PermissionController(IPermissionService services, IMapper mapper)
        {
            this.mapper = mapper;
            this.services = services;
        }

        [HttpGet("api/v1/permission")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDTO>>>> AllPermission(int? id)
        {
            try
            {

                var permissions = await services.GetAllPermissionsAsync(id);

                var perDto = mapper.Map<IEnumerable<PermissionDTO>>(permissions);

                var response = ApiResponse<IEnumerable<PermissionDTO>>.Success(perDto, "Permissions fetched successfully.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<PermissionDTO>>.Failure($"An error occurred fetching permissions: {ex.Message}");
                return StatusCode(500, response);
            }
        }

        [HttpPost("api/v1/permission")]
        public async Task<ActionResult> AddPermission(PermissionDTO permissionDTO)
        {

            try
            {
                var permission = await services.CreatePermissionAsync(permissionDTO);
                var response = ApiResponse<PermissionDTO>.Success(mapper.Map<PermissionDTO>(permission));
                return Ok(response);

            }
            catch (Exception ex)
            {
                
                    var response = ApiResponse<PermissionDTO>.Failure($"Error in Created {ex.Message}");
                    return StatusCode(500, response);
                
            }
        }

        [HttpPut("api/v1/permissions/{id}")]
        public async Task<ActionResult<ApiResponse<PermissionDTO>>> UpdatePermission(int id, [FromBody] PermissionDTO permissionDTO)
        {
            try
            {
                var updatedPermission = await services.UpdatePermissionAsync(id, permissionDTO);

                if (updatedPermission == null)
                {
                    return NotFound(ApiResponse<PermissionDTO>.Failure("Permission not found"));
                }
  
                var mappedDto = mapper.Map<PermissionDTO>(updatedPermission);
                var response = ApiResponse<PermissionDTO>.Success(mappedDto, "Update Done");

                return Ok(response);
            }
            catch (Exception ex)
            {
              
                var response = ApiResponse<PermissionDTO>.Failure($"Failed to update permission: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("/api/v1/permission/{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var success = await services.DeletePermissionAsync(id);

                if (!success)
                {
                    var notFoundResponse = ApiResponse<object>.Failure("Permission not found");
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<object>.Success(null, "Permission deleted successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                
                var errorResponse = ApiResponse<object>.Failure($"Failed to delete permission: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

    }
}