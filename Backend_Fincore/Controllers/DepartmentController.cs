using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Department;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace Backend_Fincore.Controllers
{
    [Authorize]
    [Route("api/v1/department")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class DepartmentController : ControllerBase
    {
        IDepartmentService service;

        public DepartmentController(IDepartmentService service) {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalRecordDepartment();
            var totalPages = (int)Math.Ceiling( totalRecords /(double)pagination.PageSize);
            return Ok(
                new ApiResponse<List<DepartmentReadDTO>>
                {
                    Success = true,
                    Message = "Departments fetched successfully.",
                    Data = res,
                    Error = null,
                    Metadata = new
                    {
                        pagination.PageNumber,
                        pagination.PageSize,
                        pagination.Search,
                        TotalPages = totalPages,
                        RecordsOnCurrentPage = res.Count
                    },
                    TotalNumberRecord=totalRecords
                });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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


        [HttpPost]
        public async Task<IActionResult> AddDepartment( DepartmentWriteDTO dto)
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
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDTO dto)
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
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
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDepartmentDropdown(string? search)
        {
            var res = await service.GetDepartmentDropdown(search);

            return Ok(
                new ApiResponse<List<DepartmentDropdownDTO>>
                {
                    Success = true,
                    Message = "Departments fetched successfully.",
                    Data = res,
                    Error = null,
                    TotalNumberRecord = res.Count
                });
        }
    }
}
