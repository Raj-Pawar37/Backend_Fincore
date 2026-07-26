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
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class CapexRequestController : ControllerBase
    {
        private readonly ICapexRequestService capexService;

        public CapexRequestController(ICapexRequestService capexService)
        {
            this.capexService = capexService;
        }

        [HttpGet("budget-line-dropdown")]
        public async Task<IActionResult> GetBudgetLineDropdown(
            string? searchText,
            int? departmentId)
        {
            var data = await capexService.GetBudgetLineDropdown(searchText, departmentId);

            return Ok(new ApiResponse<List<BudgetLineDropdownDTO>>
            {
                Success = true,
                Message = "Budget lines fetched successfully.",
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddCapexRequest(CapexWriteDTO dto)
        {
            var data = await capexService.AddCapexRequest(dto);

            return Ok(new ApiResponse<CapexReadDTO>
            {
                Success = true,
                Message = "CAPEX request added successfully.",
                Data = data
            });
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var data = await capexService.GetAll(pagination);

            var totalRecords = await capexService.GetTotalRecord();

            var totalPages = (int)Math.Ceiling(
                totalRecords /
                (double)pagination.PageSize);

            return Ok(new ApiResponse<List<CapexReadDTO>>
            {
                Success = true,
                Message = "CAPEX requests fetched successfully.",
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

        [HttpGet("{capexRequestId}")]
        public async Task<IActionResult> GetById(int capexRequestId)
        {
            var data = await capexService.GetById(capexRequestId);

            return Ok(new ApiResponse<CapexReadDTO>
            {
                Success = true,
                Message = "CAPEX request fetched successfully.",
                Data = data
            });
        }

        [HttpPut("{capexRequestId}")]
        public async Task<IActionResult> UpdateCapexRequest(int capexRequestId,CapexWriteDTO dto)
        {
            var data = await capexService.UpdateCapexRequest(capexRequestId,dto);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "CAPEX request updated successfully.",
                Data = data
            });
        }

        [HttpDelete("{capexRequestId}")]
        public async Task<IActionResult> DeleteCapexRequest(
            int capexRequestId,
            int userId)
        {
            var data = await capexService.DeleteCapexRequest(capexRequestId, userId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "CAPEX request deleted successfully.",
                Data = data
            });
        }

        [HttpPut("verify")]
        public async Task<IActionResult> VerifyCapexRequest(CapexVerifyDTO dto)
        {
            var data = await capexService.VerifyCapexRequest(dto);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "CAPEX request verified successfully.",
                Data = data
            });
        }
    }
}