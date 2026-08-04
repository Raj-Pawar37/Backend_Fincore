using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrder;

using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Authorize]
    public class purchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService purchaseOrderService;

        public purchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            this.purchaseOrderService = purchaseOrderService;
        }

      
        [HttpGet("GetAllPurchaseOrders")]
        public async Task<IActionResult> GetAllPurchaseOrders([FromQuery] PaginationDTO pagination)
        {
            var data = await purchaseOrderService.GetAllPurchasedOrder(pagination);

            var totalRecords = await purchaseOrderService.GetPurchasedOrderCount();
            var totalPages = (int)Math.Ceiling(
                                                totalRecords /
                                                (double)pagination.PageSize);

            if (data.Any())
            {

                return Ok(new ApiResponse<List<PurchaseOrderDTO>>
                {
                    Success = true,
                    Message = "Purchase Orders fetched successfully.",
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
            else
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "No data found.",
                   
                });

            }
        }

       
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

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, PurchaseOrderCUDTO dto)
        {
            await purchaseOrderService.UpdatePurchaseOrder(dto, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order updated successfully.",
               
            });
        }

      
        [HttpPut("Status/{id}")]
        public async Task<IActionResult> UpdatePurchaseOrderStatus(int id, PurchasedOrderStatusDTO dto)
        {
            await purchaseOrderService.UpdatePOStatus(id, dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order status updated successfully.",
               
            });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            await purchaseOrderService.DeletePurchaseOrderById(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order deleted successfully.",
               
            });
        }

        [HttpGet]
        public async Task<IActionResult> FetchIssued()
        {
            var data = await purchaseOrderService.FetchIssuedPO();

            return Ok(new ApiResponse<List<PurchaseOrderDTO>>
            {
                Success = true,
                Message = "Purchase Orders fetched successfully.",
                Data = data,
                Error = null

            });

        }
    }
}
