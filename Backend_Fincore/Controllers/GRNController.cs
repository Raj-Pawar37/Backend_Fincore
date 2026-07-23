using Backend_Fincore.Application.DTOs.GRN;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Backend_Fincore.Service;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GRNController : ControllerBase
    {
        private readonly IGRNService gRNService;

        public GRNController(IGRNService gRNService)
        {
            this.gRNService = gRNService;
        }


        [HttpGet]
        public async Task<IActionResult> getAllGRNs(GrnStatusDTO dto)
        {
            var masterType = User.FindFirst("masterType")?.Value;
            var masterId = int.Parse(User.FindFirst("masterId")!.Value);

            var data = await gRNService.GetAllGrns(masterType!, masterId, dto);

            return Ok(new ApiResponse<List<GRNDTO>>
            {
                Success = true,
                Message = "GRN list fetched successfully.",
                Data = data,
                Error = null,
                Metadata = null,
                TotalNumberRecord = data.Count
            });

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
                    Data = null,
                    Error = $"No GRN found with Id {id}.",
                     Metadata = null,
                    TotalNumberRecord = 0
                });
            }

            return Ok(new ApiResponse<GRNDTO>
            {
                Success = true,
                Message = "GRN fetched successfully.",
                Data = grn,
                Error = null,
                 Metadata = null,
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
                Message = "GRN created successfully.",
                Data = null,
                Error = null,
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

            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await gRNService.UpdateGRN(grn, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN updated successfully.",
                Data = null,
                Error = null,
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
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }


        [HttpPut("{id}/Status")]
       
        public async Task<IActionResult> UpdateGRNStatus(int id, GrnStatusDTO dto)
        {
            //int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await gRNService.UpdateGRNStatus(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN status updated successfully.",
                Data = null,
                Error = null,
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
