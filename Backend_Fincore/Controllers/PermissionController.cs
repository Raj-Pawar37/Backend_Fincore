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

        public PermissionController(IPermissionService services , IMapper mapper)
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

    }
}
