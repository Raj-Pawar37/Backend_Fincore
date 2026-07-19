using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.DTOs.PurchaseOrderItem;
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
    public class PurchaseOrderItemController : ControllerBase
    {
        private readonly IPurchaseOrderItemService purchaseOrderItemService;

        public PurchaseOrderItemController(IPurchaseOrderItemService purchaseOrderItemService)
        {
            this.purchaseOrderItemService = purchaseOrderItemService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPurchasedItem()
        {
            var itemList = await purchaseOrderItemService.getAllItem();

            return Ok(new ApiResponse<List<PurchaseOrderItemDTO>>
            {
                Success = true,
                Message = "Purchase Items fetched successfully.",
                Data = itemList,
                Error = null
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemByid(int id)
        {
            var item = await purchaseOrderItemService.getItemById(id);



            if (item == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Purchase Item not found.",
                    Data = null,
                    Error = $"No Purchase Item found with Id {id}."
                });


            }

            return Ok(new ApiResponse<PurchaseOrderItemDTO>
            {
                Success = true,
                Message = "Purchase Order fetched successfully.",
                Data = item,
                Error = null
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
                Message = "Purchase Item updated successfully.",
                Data = null,
                Error = null
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteItemById(int id)
        {
            var data = await purchaseOrderItemService.DeleteItem(id);

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Purchase Item not found.",
                    Data = null,
                    Error = $"No Purchase Order Item found with Id {id}."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order Item deleted successfully.",
                Data = null,
                Error = null
            });
        }





            
    }
}
