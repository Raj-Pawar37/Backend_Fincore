using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.GRN;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class gRNController : ControllerBase
    {
        private readonly IGRNService gRNService;

        public gRNController(IGRNService gRNService)
        {
            this.gRNService = gRNService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllGRNs([FromBody]GrnStatusDTO dto,[FromQuery]PaginationDTO pagination)
        {
            var data = await gRNService.GetAllGrns( dto,pagination);

            var totalCounts = await gRNService.GetAllGRNCount();
            var totalpages = (int)Math.Ceiling(totalCounts / (double)pagination.PageSize);

            if (data != null)
            {
                return Ok(new ApiResponse<List<GRNDTO>>
                {
                    Success = true,
                    Message = "GRN list fetched successfully.",
                    Data = data,
                    Error = null,
                    TotalNumberRecord = totalCounts,
                    Metadata = new
                    {
                        pagination.PageNumber,
                        pagination.PageSize,
                        pagination.Search,
                        TotalPages = totalpages,
                        RecordsOnCurrentPage = data.Count
                    }
                });
            }
            else
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "GRN items not found.",
                   
                });

            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getGrnById(int id)
        {
            var grn = await gRNService.GetGrnById(id);

            if (grn == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "GRN not found.",
                    
                });
            }

            return Ok(new ApiResponse<GRNDTO>
            {
                Success = true,
                Message = "GRN fetched successfully.",
                Data = grn,
                TotalNumberRecord = 1
            });
        }


        [HttpPost]
        public async Task<IActionResult> addGrn(GRNCUDTO grn)
        {
            await gRNService.AddGrn(grn);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN created successfully",
                Metadata = new
                {
                    GRNNumber = grn.GRNNumber,
                    PurchaseOrderId = grn.PurchaseOrderId,
                    Status = "Draft"
                },
                TotalNumberRecord = 1
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateGrn(GRNCUDTO grn,int id)
        { 
            await gRNService.UpdateGRN(grn, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN updated successfully.",
                Metadata = new
                {   
                    GRNId = id,
                    GRNNumber = grn.GRNNumber
                },
                TotalNumberRecord = 1
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteById(int id)
        {

            await gRNService.DeletegrnById(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN deleted successfully.",
                
            });
        }


        [HttpPut]
        [Route("Status/{id}")]
       
        public async Task<IActionResult> UpdateGRNStatus(int id, GrnStatusDTO dto)
        { 
            await gRNService.UpdateGRNStatus(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN status updated successfully.",
                Metadata = new
                {
                    GRNId = id,
                    Status = dto.Status
                },
                TotalNumberRecord = 1
            });
        }


    }
}
