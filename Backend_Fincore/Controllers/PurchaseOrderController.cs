using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.PurchaseOrderItem;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService purchaseOrderService;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
        {
            this.purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllPurchaseOrder()
        {
            var orderList = await purchaseOrderService.GetAllPurchasedOrder();

            return Ok(new ApiResponse<List<PurchaseOrderDTO>>
            {
                Success = true,
                Message = "Purchase Orders fetched successfully.",
                Data = orderList,
                Error = null
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getPurchasedOrderById(int id)
        {
            var data = await purchaseOrderService.GetPurchaseOrderById(id);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Purchase Order not found.",
                    Data = null,
                    Error = $"No Purchase Order found with Id {id}."
                });


            }

            return Ok(new ApiResponse<PurchaseOrderDTO>
            {
                Success = true,
                Message = "Purchase Order fetched successfully.",
                Data = data,
                Error = null
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteById(int id)
        {
            var data = await purchaseOrderService.DeletePurchaseOrderById(id);

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Purchase Order not found.",
                    Data = null,
                    Error = $"No Purchase Order found with Id {id}."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order deleted successfully.",
                Data = null,
                Error = null
            });
        }


        [HttpPost]
        public async Task<IActionResult> addPurchaseOrder(PurchaseOrderCUDTO PO)
        {
            await purchaseOrderService.AddPurchaseOrderData(PO);

            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order created successfully.",
                Data = null,
                Error = null
            });


        }


        [HttpPut("{id}")]
        public async Task<IActionResult> updatePurchaseOrder(PurchaseOrderCUDTO dto,int id)
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await purchaseOrderService.UpdatePurchaseOrder(dto,id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order updated successfully.",
                Data = null,
                Error = null
            });
        }


        [HttpPost("CreateFromQuotation")]
        public async Task<IActionResult> CreatePOFromQuotation(SelectedQuotationDTO QuoDto)
        {
            await purchaseOrderService.CreatePOFromQuotation(QuoDto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order created successfully from quotation.",
                Data = null,
                Error = null
            });
        }


        [HttpGet("DepartmentPurchaseOrders/{userId}")]
        public async Task<IActionResult> GetPOByDepartmentWise(int userid)
        {
            var data = await purchaseOrderService.GetDepartmentPurchaseOrders(userid);

            return Ok(new ApiResponse<List<PurchaseOrderDTO>>
            {
                Success = true,
                Message = "Department Purchase Orders fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPut("{id}/Issue")]

        public async Task<IActionResult> IssuePurchaseOrder(int id, UpdatePoStatusDTO updateStatus)
        {
            await purchaseOrderService.UpdatePOStatus(id, updateStatus);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase Order status updated successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpGet("VendorIssuedPO/{vendorId}")]
        public async Task<IActionResult> VendorIssuedPO(int vendorId)
        {
            var data = await purchaseOrderService.GetVendorIssuedPurchaseOrders(vendorId);

            if (data == null || !data.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No Issued Purchase Orders found.",
                    Data = null,
                    Error = $"No issued Purchase Orders available for Vendor Id {vendorId}."
                });
            }

            return Ok(new ApiResponse<List<PurchaseOrderDTO>>
            {
                Success = true,
                Message = "Issued Purchase Orders fetched successfully.",
                Data = data,
                Error = null
            });
        }
    }
}
