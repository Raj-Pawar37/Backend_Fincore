using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.Infrastucture.Service;
using Backend_Fincore.Service;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetsService assetsService;

        public AssetsController(IAssetsService assetsService)
        {
            this.assetsService = assetsService;
        }


        [HttpGet]
        public async Task<IActionResult> getAllAssets()
        {
            var data = await assetsService.GetAllAssetsList();

            return Ok(new ApiResponse<List<AssetsDTO>>
            {
                Success = true,
                Message = "Assets fetched successfully.",
                Data = data,
                Error = null
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> getAssetsById(int id)
        {
            var data = await assetsService.GetAssetById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "AP Invoice not found.",
                    Data = null,
                    Error = $"No AP Invoice found with Id {id}."
                });
            }

            return Ok(new ApiResponse<AssetsDTO>
            {
                Success = true,
                Message = "AP Invoice fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> addAsset(AssetsCUDTO asset)
        {
            await assetsService.AddAssets(asset);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Asset created successfully.",
                Data = null,
                Error = null
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> updateAsset(int id, AssetsCUDTO assetsDto)
        {
            await assetsService.UpdateAssets(assetsDto, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Asset updated successfully.",
                Data = null,
                Error = null
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteAssetsById(int id)
        {
            var data = await assetsService.DeleteAssetsByid(id);

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Asset not found.",
                    Data = null,
                    Error = $"No Asset found with Id {id}."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Asset deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}
