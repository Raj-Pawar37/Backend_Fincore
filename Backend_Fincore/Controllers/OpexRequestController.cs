using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.OpexRequest;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class OpexRequestController : ControllerBase
    {
        private readonly IOpexRequestService service;

        public OpexRequestController(IOpexRequestService service)
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
                .GetOpexRequestCount(userId, pagination);

            var totalPages = (int)Math.Ceiling(
                totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<OpexRequestReadDTO>>
            {
                Success = true,
                Message = "OPEX Requests fetched successfully.",
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
        public async Task<IActionResult> Create(
            OpexRequestWriteDTO dto)
        {
        
                var data = await service.Create(dto);
            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Opex Request Data not found.",
                    Data = null,
                    Error = $"No Opex Request Data found "
                });
            }

            return Ok(new ApiResponse<OpexRequestReadDTO>
            {
                Success = true,
                Message = "Opex Request created successfully.",
                Data = data,
                Error = null
            });

        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
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
        public async Task<IActionResult> Delete(int id)
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
            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Verify  Request not found.",
                    Data = null,
                    Error = $"No Verify Request found with Id = {id}"
                });
            }
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