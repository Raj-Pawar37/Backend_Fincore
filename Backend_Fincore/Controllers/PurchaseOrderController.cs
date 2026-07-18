using Backend_Fincore.Interface;
using Backend_Fincore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllPurchaseOrder()
        {
            var orderList = await purchaseOrderService.GetAllPurchasedOrder();

            return Ok(new {data = orderList});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchasedOrderById(int id)
        {
            var data = await purchaseOrderService.GetPurchaseOrderById(id);

            if (data == null)
            {
                return NotFound(new
                {
                    Message = "Purchase Order not found."
                });


            }

            return Ok(new { data = data });
        }
    }
}
