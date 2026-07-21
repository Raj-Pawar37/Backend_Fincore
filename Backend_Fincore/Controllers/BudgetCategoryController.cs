using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetCategoryController : ControllerBase
    {
        private readonly IBudgetCategoryService service;

        public BudgetCategoryController(IBudgetCategoryService service)
        {
            this.service = service;
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
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAll();

            return Ok(new ApiResponse<List<BudgetCategoryReadDTO>>
            {
                Success = true,
                Message = "Budget Categories retrieved successfully.",
                Data = data
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
        public async Task<IActionResult> UpdateBudgetCategory(int id, BudgetCategoryWriteDTO dto)
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