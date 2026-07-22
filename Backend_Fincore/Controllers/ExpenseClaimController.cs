using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseClaimController : ControllerBase
    {
        private readonly IExpenseClaimService service;

        public ExpenseClaimController(
            IExpenseClaimService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExpenseClaims()
        {
            var data = await service.GetAll();

            return Ok(new ApiResponse<List<ExpenseClaimReadDTO>>
            {
                Success = true,
                Message = "Expense claims fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseClaimById(int id)
        {
            var data = await service.GetById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Expense claim not found.",
                    Data = null,
                    Error = $"No expense claim found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<ExpenseClaimReadDTO>
            {
                Success = true,
                Message = "Expense claim fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddExpenseClaim(
            ExpenseClaimWriteDTO dto)
        {
            var data = await service.Create(dto);

            return Ok(new ApiResponse<ExpenseClaimReadDTO>
            {
                Success = true,
                Message = "Expense claim created successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpenseClaim(
            int id,
            ExpenseClaimWriteDTO dto)
        {
            var data = await service.Update(id, dto);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Expense claim not found.",
                    Data = null,
                    Error = $"No expense claim found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<ExpenseClaimReadDTO>
            {
                Success = true,
                Message = "Expense claim updated successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseClaim(int id)
        {
            var result = await service.Delete(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Expense claim not found.",
                    Data = null,
                    Error = $"No expense claim found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Expense claim deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}