using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Interface;
using Backend_Fincore.Response;
using Backend_Fincore.Service;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("Fixed")]
    public class PurchaseOrderItemController : ControllerBase
    {
        private readonly IPurchaseOrderItemService purchaseOrderItemService;

        public PurchaseOrderItemController(IPurchaseOrderItemService purchaseOrderItemService)
        {
            this.purchaseOrderItemService = purchaseOrderItemService;
        }


        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAllPurchasedItemByRole(ReadPoItemsDTO dto,PaginationDTO pagination)
        {
            var data = await purchaseOrderItemService.getAllPurchasedItem(dto, pagination);

            var totalRecords = await purchaseOrderItemService.GetPurchasedItemCount();
            var totalPages = (int)Math.Ceiling(
                    totalRecords /
                    (double)pagination.PageSize);

            return Ok(new ApiResponse<List<PurchaseOrderItemDTO>>
            {
                Success = true,
                Message = data == null ? "PO Item not found." : "PO Item fetched successfully.",
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
        public async Task<IActionResult> getItemByid(int id)
        {
            var item = await purchaseOrderItemService.getItemById(id);

            return Ok(new ApiResponse<PurchaseOrderItemDTO>
            {
                Success = true,
                Message = "Purchase Order Item fetched successfully.",
                Data = item,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> addPurchasedItem(PurchaseOrderItemCUDTO PI)
        {


            await purchaseOrderItemService.AddPurchasedItem(PI);


            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchased Item add successfully.",
                Data = null,
                Error = null
            });

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> updatePurchasedItem(int id,PurchaseOrderItemCUDTO Pi)
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await purchaseOrderItemService.UpdatePurchaseOrderItem(Pi, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order Item updated successfully.",
                Data = null,
                Error = null,
                Metadata = new
                {
                    PurchaseOrderItemId = id,
                    ItemName = Pi.ItemName
                },
                TotalNumberRecord = 1
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteItemById(int id)
        {
            var data = await purchaseOrderItemService.DeleteItem(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order Item deleted successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }
        
    }
}
