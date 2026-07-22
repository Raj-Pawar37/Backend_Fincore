using Backend_Fincore.Application.DTOs.AccountMaster;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMasterController : ControllerBase
    {
        private readonly IAccountMasterService service;

        public AccountMasterController(IAccountMasterService service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await service.GetAll();

            return Ok(new ApiResponse<List<AccountMasterReadDTO>>
            {
                Success = true,
                Message = "Account Masters fetched successfully.",
                Data = res,
                Error = null
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
