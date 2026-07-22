using Backend_Fincore.Application.DTOs.OpexRequest;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpexRequestController : ControllerBase
    {
        private readonly IOpexRequestService service;

        public OpexRequestController(IOpexRequestService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var data = await service.GetAll();

            return Ok(new ApiResponse<List<OpexRequestReadDTO>>
            {
                Success = true,
                Message = "Opex Requests fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var data = await service.GetById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Opex Request not found.",
                    Data = null,
                    Error = $"No Opex Request found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<OpexRequestReadDTO>
            {
                Success = true,
                Message = "Opex Request fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddOpexRequest(OpexRequestWriteDTO dto)
        {
            await service.Create(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Opex Request created successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpexRequest(int id, OpexRequestWriteDTO dto)
        {
            var result = await service.Update(id, dto);

            if (result == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Opex Request not found.",
                    Data = null,
                    Error = $"No Opex Request found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Opex Request updated successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpexRequest(int id)
        {
            var result = await service.Delete(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Opex Request not found.",
                    Data = null,
                    Error = $"No Opex Request found with Id = {id}"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Opex Request deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}