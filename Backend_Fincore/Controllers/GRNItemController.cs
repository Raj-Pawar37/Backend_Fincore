using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GRNItemController : ControllerBase
    {
        private readonly IGRNItemsService gRNItemsService;

        public GRNItemController(IGRNItemsService gRNItemsService)
        {
            this.gRNItemsService = gRNItemsService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllGRNItems()
        {
            var data = await gRNItemsService.getAllGrnItems();

            return Ok(new ApiResponse<List<GRNItemsDTO>>
            {
                Success = true,
                Message = "GRN Items fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = data.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGRNItemById(int id)
        {
            var data = await gRNItemsService.GetGRNItemById(id);

            return Ok(new ApiResponse<GRNItemsDTO>
            {
                Success = true,
                Message = "GRN Item fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = 1
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGRNItem(int id)
        {
            await gRNItemsService.DeleteGRNItem(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "GRN Item deleted successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }
    }
}
