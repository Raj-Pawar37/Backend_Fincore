using Backend_Fincore.Application.DTOs.Department;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        IDepartmentService service;
        public DepartmentController(IDepartmentService service) {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await service.GetAll();
            return Ok(
                new ApiResponse<List<DepartmentReadDTO>>
                {
                    Success = true,
                    Message = "Departments fetched successfully.",
                    Data = res,
                    Error = null
                });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var res = await service.GetById(id);
                return Ok(
                    new ApiResponse<DepartmentReadDTO>
                    {
                        Success = true,
                        Message = "Department fetched successfully.",
                        Data = res,
                        Error = null
                    });
            }
            catch (Exception ex)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found.",
                        Data = null,
                        Error = ex.Message
                    });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddDepartment(
            DepartmentWriteDTO dto)
        {
            var res = await service.AddDepartment(dto);
            return Ok(
                new ApiResponse<DepartmentReadDTO>
                {
                    Success = true,
                    Message = "Department created successfully.",
                    Data = res,
                    Error = null
                });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentWriteDTO dto)
        {
            try
            {
                await service.UpdateDepartment( id, dto);
                return Ok(
                    new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Department updated successfully.",
                        Data = null,
                        Error = null
                    });
            }
            catch (Exception ex)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found.",
                        Data = null,
                        Error = ex.Message
                    });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                await service.DeleteDepartment(id);
                return Ok(
                    new ApiResponse<object>
                    {
                        Success = true,
                        Message = "Department deleted successfully.",
                        Data = null,
                        Error = null
                    });
            }
            catch (Exception ex)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found.",
                        Data = null,
                        Error = ex.Message
                    });
            }
        }

    }
}
