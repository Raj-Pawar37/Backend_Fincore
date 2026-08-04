using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.DTOs;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Authorize]
    //[Route("api/v1/[controller]")]
    [Route("api/v1/budgetCategory")]
    [ApiController]
    [EnableRateLimiting("fixed")]

    public class BudgetCategoryController : ControllerBase
    {
        private readonly IBudgetCategoryService service;

        public BudgetCategoryController(IBudgetCategoryService service)
        {
            this.service = service;
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetBudgetCategoryDropdown([FromQuery] string? search)
        {
            var data = await service.GetBudgetCategoryDropdown(search);

            return Ok(new ApiResponse<List<BudgetCategoryDropdownDTO>>
            {
                Success = true,
                Message = "Budget Category dropdown fetched successfully.",
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddBudgetCategory(BudgetCategoryWriteDTO dto)
        {
            var data = await service.AddBudgetCategory(dto);

            return Ok(new ApiResponse<BudgetCategoryReadDTO>
            {
                Success = true,
                Message = "Budget Category created successfully.",
                Data = data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDTO pagination)
        {
            var data = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalRecord();
            var totalPages = (int)Math.Ceiling(
                    totalRecords /
                    (double)pagination.PageSize);

            return Ok(new ApiResponse<List<BudgetCategoryReadDTO>>
            {
                Success = true,
                Message = "Budget Categories retrieved successfully.",
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
                    Message = "Budget Category not found."
                });
            }

            return Ok(new ApiResponse<BudgetCategoryReadDTO>
            {
                Success = true,
                Message = "Budget Category retrieved successfully.",
                Data = data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudgetCategory(int id, BudgetCategoryUpdateDTO dto)
        {
            var result = await service.UpdateBudgetCategory(id, dto);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget Category not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Budget Category updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudgetCategory(int id)
        {
            var result = await service.DeleteBudgetCategory(id);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Budget Category not found."
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Budget Category deleted successfully."
            });
        }
    }
}