using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.API.Controllers
{
    [Route("api/v1/rfqs")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class RFQsController : ControllerBase
    {
        private readonly IRFQService _rfqService;

        public RFQsController(IRFQService rfqService)
        {
            _rfqService = rfqService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRFQs([FromQuery] PaginationDTO pagination)
        {
            var data = await _rfqService.GetAllAsync(pagination);

            if (data == null || data.Count == 0)
            {
                return Ok(new ApiResponse<List<RFQResponseDto>>
                {
                    Success = false,
                    Message = "Data does not exist.",
                    Data = new List<RFQResponseDto>(),
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            var totalRecords = await _rfqService.GetCountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<RFQResponseDto>>
            {
                Success = true,
                Message = "RFQs fetched successfully.",
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
        public async Task<IActionResult> GetRFQById(int id)
        {
            var data = await _rfqService.GetByIdAsync(id);

            if (data == null)
            {
                return Ok(new ApiResponse<RFQResponseDto>
                {
                    Success = false,
                    Message = "Data does not exist for this id.",
                    Data = null,
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            return Ok(new ApiResponse<RFQResponseDto>
            {
                Success = true,
                Message = "RFQ fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddRFQ(RFQCreateDto dto)
        {
            await _rfqService.CreateAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ created successfully.",
                Data = null,
                Error = null,
                Metadata = new
                {
                    RFQNumber = dto.RFQNumber,
                    PRId = dto.PRId,
                    Status = "Pending"
                },
                TotalNumberRecord = 1
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRFQ(int id, RFQUpdateDto dto)
        {
            await _rfqService.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ updated successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRFQ(int id)
        {
            await _rfqService.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ deleted successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }
    }
}