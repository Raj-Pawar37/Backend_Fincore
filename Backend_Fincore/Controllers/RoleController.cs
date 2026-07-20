
using AutoMapper;
using Backend_Fincore.Data;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        AppDbContext db;
        private readonly IMapper mapper;
        private readonly IRoleService _roleService;

      
        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            this.mapper = mapper;
        }

        [HttpGet("/api/v1/roles")]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDTO>>>> AllRoles()
        {
            try
            {
                //var roles = await _roleService.GetAllRolesAsync();
                //var mapRole = mapper.Map<IEnumerable<RoleDTO>>(roles);
                //var response = ApiResponse<IEnumerable<RoleDTO>>.Success(mapRole, "Roles retrieved successfully");
                //return Ok(response);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                //var response = ApiResponse<IEnumerable<RoleDTO>>.Failure($"An error occurred while retrieving roles: {ex.Message}");

                //return StatusCode(500, response);
                throw new NotImplementedException();
            }
        }


        [HttpGet("/api/v1/roles/{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            try
            {
                //var role = await _roleService.GetRoleByIdAsync(id);
                //var roleDTO = mapper.Map<RoleDTO>(role);
                //var response = ApiResponse<RoleDTO>.Success(roleDTO, "Role retrieved successfully");
                //if (role == null)
                //{
                //    return NotFound(new { message = "Role not found" });
                //}
                //return Ok(response);
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                //var response = ApiResponse<RoleDTO>.Failure($"An error occurred while retrieving the role: {ex.Message}");
                //return StatusCode(500, response);
                throw new NotImplementedException();
            }
        }

        [HttpPost("/api/v1/roles")]
        public async Task<ActionResult<Role>> CreateRole(RoleDTO dto)
        {
            //try
            //{
            //    var createdRole = await _roleService.CreateRoleAsync(dto);
            //    var response = ApiResponse<RoleDTO>.Success(mapper.Map<RoleDTO>(createdRole), "Role created successfully");
            //    return Ok(response);
            //}
            //catch (Exception ex)
            //{
            //    var response = ApiResponse<RoleDTO>.Failure($"An error occurred while retrieving the role: {ex.Message}");
            //    return StatusCode(500, response); return StatusCode(500, new { message = $"An error occurred while creating the role: {ex.Message}" });
            //}

            throw new NotImplementedException();
        }

        [HttpPut("/api/v1/roles/{id}")] 
        public async Task<ActionResult<Role>> UpdateRole(int id, RoleDTO dto)
        {
            //try
            //{
            //    var updatedRole = await _roleService.UpdateRoleAsync(id, dto);
            //    if (updatedRole == null)
            //    {
            //        return NotFound(new { message = "Role not found" });
            //    }
            //    var response = ApiResponse<RoleDTO>.Success(mapper.Map<RoleDTO>(updatedRole), "Role updated successfully");
            //    return Ok(response);
            //}
            //catch(Exception ex)
            //{
            //    var response = ApiResponse<RoleDTO>.Failure($"An error occurred while updating the role: {ex.Message}");
            //    return StatusCode(500, response);

            //}

            throw new NotImplementedException();
        }

        [HttpDelete("/api/v1/roles/{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            //var success = await _roleService.DeleteRoleAsync(id);
            //var response = ApiResponse<object>.Success(null, "Role deleted successfully");
            //if (!success)
            //{
            //    return NotFound(new { message = "Role not found" });
            //}
            //return Ok(response);


            throw new NotImplementedException();
        }
    }
}
