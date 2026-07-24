using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService service;

        public EmployeeController(IEmployeeService service)
        {
            this.service = service;
        }

        [HttpGet]
        // public async Task<IActionResult> GetAll([FromQuery]PaginationDTO pagination)
        public async Task<IActionResult> GetAll()
        {
            var res = await service.GetAll();
            //var totalRecords = await service.GetTotalEmployeeRecords();
            //var totalPages = (int)Math.Ceiling(
            //      totalRecords /
            //      (double)pagination.PageSize);

            return Ok(new ApiResponse<List<EmployeeReadDTO>>
            {
                Success = true,
                Message = "Employees fetched successfully.",
                Data = res,
                Error = null,
                //TotalNumberRecord = totalRecords,
                //Metadata = new
                //{
                //    pagination.PageNumber,
                //    pagination.PageSize,
                //    pagination.Search,
                //    TotalPages = totalPages,
                //    RecordsOnCurrentPage = res.Count
                //}
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
                    Message = "Employee not found.",
                    Data = null,
                    Error = $"No employee found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<EmployeeReadDTO>
            {
                Success = true,
                Message = "Employee fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeWriteDTO dto)
        {
            var data = await service.AddEmp(dto);

            return Ok(new ApiResponse<EmployeeReadDTO>
            {
                Success = true,
                Message = "Employee created successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeWriteDTO dto)
        {
            var result = await service.update(id, dto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Employee not found.",
                    Data = null,
                    Error = $"No employee found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Employee updated successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await service.delete(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Employee not found.",
                    Data = null,
                    Error = $"No employee found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Employee deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}