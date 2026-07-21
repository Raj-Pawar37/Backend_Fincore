using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetLineController : ControllerBase
    {
        private readonly IBudgetLineService service;

        public BudgetLineController(IBudgetLineService service)
        {
            this.service = service;
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
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAll();

            return Ok(new ApiResponse<List<BudgetLineReadDTO>>
            {
                Success = true,
                Message = "Budget lines retrieved successfully.",
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