using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
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
    [Authorize]
    [EnableRateLimiting("fixed")]
    [Route("api/v1/purchaseRequisitions")]
    [ApiController]
    public class PurchaseRequisitionsController : ControllerBase
    {
        private readonly IPurchaseRequisitionService _prService;

        public PurchaseRequisitionsController(IPurchaseRequisitionService prService)
        {
            _prService = prService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var data = await _prService.GetAllAsync(pagination);

            // LOGICAL CHECK: Is the list empty?
            if (data == null || data.Count == 0)
            {
                return Ok(new ApiResponse<List<PurchaseRequisitionResponseDto>>
                {
                    Success = false,
                    Message = "Data does not exist.",
                    Data = new List<PurchaseRequisitionResponseDto>(),
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            var totalRecords = await _prService.GetCountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

            return Ok(new ApiResponse<List<PurchaseRequisitionResponseDto>>
            {
                Success = true,
                Message = "Purchase Requisitions fetched successfully.",
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
            var data = await _prService.GetByIdAsync(id);

            // LOGICAL CHECK: Is the single item null?
            if (data == null)
            {
                return Ok(new ApiResponse<PurchaseRequisitionResponseDto>
                {
                    Success = false,
                    Message = "Data does not exist for this id.",
                    Data = null,
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            return Ok(new ApiResponse<PurchaseRequisitionResponseDto>
            {
                Success = true,
                Message = "Purchase Requisition fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = 1
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseRequisitionUpdateDto dto)
        {
            await _prService.UpdateAsync(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Requisition updated successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetPRDropdown([FromQuery] string? searchText, [FromQuery] int? departmentId)
        {
            var data = await _prService.GetPRDropdownAsync(searchText, departmentId);

            // LOGICAL CHECK: Is the dropdown empty?
            if (data == null || data.Count == 0)
            {
                return Ok(new ApiResponse<List<PRDropdownResponseDto>>
                {
                    Success = false,
                    Message = "No Dropdown data available.",
                    Data = new List<PRDropdownResponseDto>(),
                    Error = null,
                    Metadata = new { },
                    TotalNumberRecord = 0
                });
            }

            return Ok(new ApiResponse<List<PRDropdownResponseDto>>
            {
                Success = true,
                Message = "PR Dropdown fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = data.Count
            });
        }
    }
}