using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{ 
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class UserController : ControllerBase
{
    private readonly IUserService service;

    public UserController(IUserService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
    {
        var res = await service.GetAll(pagination);
       
         var totalRecords = await service.GetTotalUserRecords(pagination.Search);
         var totalPages = (int)Math.Ceiling( totalRecords /(double)pagination.PageSize);
           
            if (!res.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No users found.",
                    Data = null,
                    Error = pagination.Search != null
                        ? $"No user found for search '{pagination.Search}'."
                        : "No users available."
                });
            }

       return Ok(new ApiResponse<List<UserReadDTO>>
        {
            Success = true,
            Message = "Users fetched successfully.",
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
    public async Task<IActionResult> GetById(int id)
    {
        var data = await service.GetById(id);

        if (data == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found.",
                Data = null,
                Error = $"No user found with Id = {id}"
            });
        }

        return Ok(new ApiResponse<UserReadDTO>
        {
            Success = true,
            Message = "User fetched successfully.",
            Data = data,
            Error = null
        });
    }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateDTO dto)
        {
            var data = await service.AddUser(dto);

            return Ok(new ApiResponse<UserReadDTO>
            {
                Success = true,
                Message = "User created successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDTO dto)
        {
            var result = await service.UpdateUser(id, dto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null,
                    Error = $"No user found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User updated successfully.",
                Data = null,
                Error = null
            });
        }


        [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await service.DeleteUser(id);

        if (!result)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "User not found.",
                Data = null,
                Error = $"No user found with Id = {id}"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "User deleted successfully.",
            Data = null,
            Error = null
        });
    }
}
}