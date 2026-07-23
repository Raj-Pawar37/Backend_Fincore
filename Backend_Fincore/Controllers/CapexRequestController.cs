using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapexRequestController : ControllerBase
    {
        private readonly ICapexRequestService capexService;

        public CapexRequestController(
            ICapexRequestService capexService)
        {
            this.capexService = capexService;
        }

        [HttpGet("budget-line-dropdown")]
        public async Task<IActionResult> GetBudgetLineDropdown(
            string? searchText,
            int? departmentId)
        {
            try
            {
                var data =
                    await capexService.GetBudgetLineDropdown(
                        searchText,
                        departmentId);

                return Ok(new ApiResponse<List<BudgetLineDropdownDTO>>
                {
                    Success = true,
                    Message = "Budget lines fetched successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<BudgetLineDropdownDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCapexRequest(
            CapexWriteDTO dto)
        {
            try
            {
                var data =
                    await capexService.AddCapexRequest(dto);

                return Ok(new ApiResponse<CapexReadDTO>
                {
                    Success = true,
                    Message = "CAPEX request added successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CapexReadDTO>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int userId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var data = await capexService.GetAll(
                    userId,
                    pageNumber,
                    pageSize);

                return Ok(new ApiResponse<List<CapexReadDTO>>
                {
                    Success = true,
                    Message = "CAPEX requests fetched successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<CapexReadDTO>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet("{capexRequestId}")]
        public async Task<IActionResult> GetById(
            int capexRequestId)
        {
            try
            {
                var data =
                    await capexService.GetById(capexRequestId);

                return Ok(new ApiResponse<CapexReadDTO>
                {
                    Success = true,
                    Message = "CAPEX request fetched successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CapexReadDTO>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("{capexRequestId}")]
        public async Task<IActionResult> UpdateCapexRequest(
            int capexRequestId,
            int userId,
            CapexWriteDTO dto)
        {
            try
            {
                var data =
                    await capexService.UpdateCapexRequest(
                        capexRequestId,
                        userId,
                        dto);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "CAPEX request updated successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [HttpDelete("{capexRequestId}")]
        public async Task<IActionResult> DeleteCapexRequest(
            int capexRequestId,
            int userId)
        {
            try
            {
                var data =
                    await capexService.DeleteCapexRequest(
                        capexRequestId,
                        userId);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "CAPEX request deleted successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }

        [HttpPut("verify")]
        public async Task<IActionResult> VerifyCapexRequest(
            CapexVerifyDTO dto)
        {
            try
            {
                var data =
                    await capexService.VerifyCapexRequest(dto);

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "CAPEX request verified successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }
    }
}