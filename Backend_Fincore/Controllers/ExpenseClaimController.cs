using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class ExpenseClaimController : ControllerBase
    {
        private readonly IExpenseClaimService service;

        public ExpenseClaimController(
            IExpenseClaimService service)
        {
            this.service = service;
        }

        [HttpGet]
     
        public async Task<IActionResult> GetAll(int userId,[FromQuery] PaginationDTO pagination)
        {
            if (pagination.PageNumber <= 0)
                pagination.PageNumber = 1;

            if (pagination.PageSize <= 0)
                pagination.PageSize = 10;

            var data = await service.GetAll(userId, pagination);

            var totalRecords = await service
                .GetExpenseClaimCount(userId, pagination);

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<ExpenseClaimReadDTO>>
            {
                Success = true,
                Message = "Expense Claims fetched successfully.",
                Data = data,
                Error = null,
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
        public async Task<IActionResult> Create(
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
        public async Task<IActionResult> Update(
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
        public async Task<IActionResult> Delete(int id)
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

        [HttpPut("{id}/verify")]
        public async Task<IActionResult> Verify(int id,int verifiedBy,ExpenseClaimVerifyDTO dto)
        {
            var data = await service.Verify(id,verifiedBy,dto);

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
                Message = $"Expense Claim {dto.Status} successfully.",
                Data = data,
                Error = null
            });
        }
    }
}