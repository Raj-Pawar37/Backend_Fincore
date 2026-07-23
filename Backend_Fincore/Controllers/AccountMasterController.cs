using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class AccountMasterController : ControllerBase
    {
        private readonly IAccountMasterService service;

        public AccountMasterController(IAccountMasterService service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords = await service.GetAccountMasterCount();
            var totalPages = (int)Math.Ceiling(
                    totalRecords /
                    (double)pagination.PageSize);

            return Ok(new ApiResponse<List<AccountMasterReadDTO>>
            {
                Success = true,
                Message = "Account Masters fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord = totalRecords,
                Metadata = new
                {
                    pagination.PageNumber,
                    pagination.PageSize,
                    pagination.Search,
                    TotalPages = totalPages,
                    RecordsOnCurrentPage = res.Count
                }

            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var res = await service.GetById(id);

                return Ok(new ApiResponse<AccountMasterReadDTO>
                {
                    Success = true,
                    Message = "Account Master fetched successfully.",
                    Data = res,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account Master not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult>AddAccountMaster(AccountMasterWriteDTO dto)
        {
            var res = await service.AddAccountMaster(dto);

            return Ok(new ApiResponse<AccountMasterReadDTO>
            {
                Success = true,
                Message = "Account Master created successfully.",
                Data = res,
                Error = null
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult>UpdateAccountMaster(int id,AccountMasterWriteDTO dto)
        {
            try
            {
                await service.UpdateAccountMaster(id, dto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Account Master updated successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account Master not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteAccountMaster(int id)
        {
            try
            {
                await service.DeleteAccountMaster(id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Account Master deleted successfully.",
                    Data = null,
                    Error = null
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account Master not found.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }

    }
}
