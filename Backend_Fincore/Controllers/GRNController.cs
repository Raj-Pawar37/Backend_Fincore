using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Interface;
using Backend_Fincore.Service;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> getAllGRNs()
        {
            var data = await gRNService.GetAllGrns();

            return Ok(new ApiResponse<List<GRNDTO>>
            {
                Success = true,
                Message = "GRNs fetched successfully.",
                Data = data,
                Error = null
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
                    Error = $"No GRN found with Id {id}."
                });
            }

            return Ok(new ApiResponse<GRNDTO>
            {
                Success = true,
                Message = "GRN fetched successfully.",
                Data = grn,
                Error = null
            });
        }


        [HttpPost]
        public async Task<IActionResult> addGrn(GRNCUDTO grn)
        {
            await gRNService.AddGrn(grn);


            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order created successfully.",
                Data = null,
                Error = null
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
                Error = null
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteById(int id)
        {
            var data = await gRNService.DeletegrnById(id);

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "GRN not found.",
                    Data = null,
                    Error = $"No GRN found with Id {id}."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN deleted successfully.",
                Data = null,
                Error = null
            });
        }


    }
}
