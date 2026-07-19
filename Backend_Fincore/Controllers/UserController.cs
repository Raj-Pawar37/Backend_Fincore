using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAll();

            return Ok(new ApiResponse<List<UserReadDTO>>
            {
                Success = true,
                Message = "Users fetched successfully.",
                Data = data,
                Error = null
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
        public async Task<IActionResult> AddUser(UserWriteDTO dto)
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
        public async Task<IActionResult> UpdateUser(int id, UserWriteDTO dto)
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