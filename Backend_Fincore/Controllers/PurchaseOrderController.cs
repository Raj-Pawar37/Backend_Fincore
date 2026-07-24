using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrder;

using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.Response;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            this.purchaseOrderService = purchaseOrderService;
        }

        // Get All Purchase Orders
        [HttpPost("GetAllPurchaseOrders")]
        public async Task<IActionResult> GetAllPurchaseOrders(PurchasedOrderFilterDTO pof)
        {
            var data = await purchaseOrderService.GetAllPurchasedOrder(pof);

            return Ok(new ApiResponse<List<PurchaseOrderDTO>>
            {
                Success = true,
                Message = "Purchase Orders fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = data.Count
            });
        }

        // Get Purchase Order By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            var data = await purchaseOrderService.GetPurchaseOrderById(id);

            return Ok(new ApiResponse<PurchaseOrderDTO>
            {
                Success = true,
                Message = "Purchase Order fetched successfully.",
                Data = data,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = 1
            });
        }

        // Create Purchase Order
        [HttpPost]
        public async Task<IActionResult> AddPurchaseOrder(PurchaseOrderCUDTO dto)
        {
            await purchaseOrderService.AddPurchaseOrderData(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order created successfully.",
                Data = null,
                Error = null,
                Metadata = new
                {
                    PurchaseOrderNumber = dto.PONumber,
                    QuotationId = dto.QuotationId,
                    Status = "Draft"
                },
                TotalNumberRecord = 1
            });
        }

        // Update Purchase Order
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, PurchaseOrderCUDTO dto)
        {
            await purchaseOrderService.UpdatePurchaseOrder(dto, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order updated successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

        // Update Purchase Order Status
        [HttpPut("{id}/Status")]
        public async Task<IActionResult> UpdatePurchaseOrderStatus(int id, PurchasedOrderFilterDTO dto)
        {
            await purchaseOrderService.UpdatePOStatus(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order status updated successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

        // Delete Purchase Order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            await purchaseOrderService.DeletePurchaseOrderById(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order deleted successfully.",
                Data = null,
                Error = null,
                Metadata = new { },
                TotalNumberRecord = null
            });
        }

    }
}
