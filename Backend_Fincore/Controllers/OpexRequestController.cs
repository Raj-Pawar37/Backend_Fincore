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
        public async Task<IActionResult> GetAll(int userId)
        {
            var data = await service.GetAll(userId);

            return Ok(new ApiResponse<List<OpexRequestReadDTO>>
            {
                Success = true,
                Message = "OPEX Requests fetched successfully.",
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
        public async Task<IActionResult> AddOpexRequest(
            OpexRequestWriteDTO dto)
        {
        
                var data = await service.Create(dto);

                return CreatedAtAction(
                    nameof(GetRequestById),
                    new { id = data.OpexRequestId },
                    new ApiResponse<OpexRequestReadDTO>
                    {
                        Success = true,
                        Message = "Opex Request created successfully.",
                        Data = data,
                        Error = null
                    });
       
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpexRequest(
            int id,
            OpexRequestWriteDTO dto)
        {
          
                var data = await service.Update(id, dto);

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
                    Message = "Opex Request updated successfully.",
                    Data = data,
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
        [HttpPut("{id}/verify")]
        public async Task<IActionResult> Verify(int id,int approvedBy,OpexRequestVerifyDTO dto)
        {
            var data = await service.Verify(id, approvedBy, dto);

            return Ok(new ApiResponse<OpexRequestReadDTO>
            {
                Success = true,
                Message = $"OPEX Request {dto.Status} successfully.",
                Data = data,
                Error = null
            });
        }


        [HttpPost("search")]
        public async Task<IActionResult> SearchOpex(OpexSearchDTO dto)
        {
            var data = await service.SearchOpex(dto);

            return Ok(new ApiResponse<List<OpexRequestReadDTO>>
            {
                Success = true,
                Message = "OPEX Requests fetched successfully.",
                Data = data,
                Error = null
            });
        }



    }
}