using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.RFQItem;
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
    [Route("api/v1/rfqItems")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class RFQItemsController : ControllerBase
    {
        private readonly IRFQItemService rfqItemService;

        public RFQItemsController(IRFQItemService rfqItemService)
        {
            this.rfqItemService = rfqItemService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RFQItemCreateDto dto)
        {
            await rfqItemService.CreateAsync(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ Item created successfully.",
                Data = null,
                Error = null,
                Metadata = new { RFQId = dto.RFQId },
                TotalNumberRecord = 1
            });
        }

        [HttpGet("{rfqId}")]
        public async Task<IActionResult> GetByRfqId(int rfqId, [FromQuery] PaginationDTO pagination)
        {
            var data = await rfqItemService.GetByRfqIdAsync(rfqId, pagination);

            // LOGICAL CHECK: Are there no items for this RFQ?
            if (data == null || data.Count == 0)
            {
                return Ok(new ApiResponse<List<RFQItemResponseDto>>
                {
                    Success = false,
                    Message = "Data does not exist for this id.",
                    Data = new List<RFQItemResponseDto>(),
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            var totalRecords = await rfqItemService.GetCountByRfqIdAsync(rfqId);
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<RFQItemResponseDto>>
            {
                Success = true,
                Message = "RFQ Items fetched successfully.",
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
        public async Task<IActionResult> Update(int id, [FromBody] RFQItemUpdateDto dto)
        {
            await rfqItemService.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ Item updated successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await rfqItemService.DeleteAsync(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ Item deleted successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }


        [HttpGet("ReadByRFQId/")]
        public async Task<IActionResult> Get([FromQuery] RFQItemReadbyRfqDTO data)
        {
            var items = await rfqItemService.ReadbyRFQId(data);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "RFQ Item Fetched successfully.",
                Data = items,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }
    }
}