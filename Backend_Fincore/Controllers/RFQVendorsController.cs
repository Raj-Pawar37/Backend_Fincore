using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQVendor;
using Backend_Fincore.Application.Interfaces;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_Fincore.API.Controllers
{
    [Route("api/v1/rfqvendors")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class RFQVendorsController : ControllerBase
    {
        private readonly IRFQVendorService rfqVendorService;

        public RFQVendorsController(IRFQVendorService rfqVendorService)
        {
            this.rfqVendorService = rfqVendorService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RFQVendorCreateDto dto)
        {
            await rfqVendorService.CreateAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Vendor added to RFQ successfully.",
                Data = null,
                Error = null,
                Metadata = new { RFQId = dto.RFQId, VendorId = dto.VendorId },
                TotalNumberRecord = 1
            });
        }

        [HttpGet("{rfqId}")]
        public async Task<IActionResult> GetByRfqId(int rfqId, [FromQuery] PaginationDTO pagination)
        {
            var data = await rfqVendorService.GetByRfqIdAsync(rfqId, pagination);

            // LOGICAL CHECK: Are there no vendors for this RFQ?
            if (data == null || data.Count == 0)
            {
                return Ok(new ApiResponse<List<RFQVendorResponseDto>>
                {
                    Success = false,
                    Message = "Data does not exist for this id.",
                    Data = new List<RFQVendorResponseDto>(),
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            var totalRecords = await rfqVendorService.GetCountByRfqIdAsync(rfqId);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<RFQVendorResponseDto>>
            {
                Success = true,
                Message = "RFQ Vendors fetched successfully.",
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RFQVendorUpdateDto dto)
        {
            await rfqVendorService.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ Vendor updated successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await rfqVendorService.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ Vendor deleted successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }
    }
}