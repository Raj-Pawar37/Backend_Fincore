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
    //[Route("api/v1/[controller]")]
    [Route("api/v1/budgetLine")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class BudgetLineController : ControllerBase
    {
        private readonly IBudgetLineService service;

        public BudgetLineController(IBudgetLineService service)
        {
            this.service = service;
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetBudgetLineDropdown(
           string? searchText,
           int? departmentId, string? costCenter)
        {
            var data = await service.GetBudgetLineDropdown(searchText, departmentId,costCenter);

            return Ok(new ApiResponse<List<BudgetLineDropdownDTO>>
            {
                Success = true,
                Message = "Budget lines fetched successfully.",
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddBudgetLine(
            BudgetLineWriteDTO dto)
        {
            var data = await service.AddBudgetLine(dto);

            return Ok(new ApiResponse<BudgetLineReadDTO>
            {
                Success = true,
                Message = "Budget line created successfully.",
                Data = data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var data = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalRecord();
            var totalPages = (int)Math.Ceiling(
                    totalRecords /
                    (double)pagination.PageSize);

            return Ok(new ApiResponse<List<BudgetLineReadDTO>>
            {
                Success = true,
                Message = "Budget lines retrieved successfully.",
                Data = data,
                TotalNumberRecord = totalRecords,
                Metadata = new
                {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = data.Count
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await service.GetById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget line not found."
                });
            }

            return Ok(new ApiResponse<BudgetLineReadDTO>
            {
                Success = true,
                Message = "Budget line retrieved successfully.",
                Data = data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudgetLine(
            int id,
            BudgetLineWriteDTO dto)
        {
            var result = await service.UpdateBudgetLine(id, dto);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget line not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Budget line updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudgetLine(int id)
        {
            var result = await service.DeleteBudgetLine(id);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget line not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Budget line deleted successfully."
            });
        }
    }
}