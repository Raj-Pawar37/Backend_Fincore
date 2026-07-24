using Backend_Fincore.Application.DTOs;
using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService service;

        public BudgetController(IBudgetService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget(BudgetWriteDTO dto)
        {
            var data = await service.AddBudget(dto);

            return Ok(new ApiResponse<BudgetReadDTO>
            {
                Success = true,
                Message = "Budget created successfully.",
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

            return Ok(new ApiResponse<List<BudgetReadDTO>>
            {
                Success = true,
                Message = "Budgets retrieved successfully.",
                Data = data,
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
                    Message = "Budget not found."
                });
            }

            return Ok(new ApiResponse<BudgetReadDTO>
            {
                Success = true,
                Message = "Budget retrieved successfully.",
                Data = data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(
            int id,
            BudgetWriteDTO dto)
        {
            var result = await service.UpdateBudget(id, dto);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Budget updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var result = await service.DeleteBudget(id);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Budget deleted successfully."
            });
        }
    }
}