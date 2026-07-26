using Backend_Fincore.Application.DTOs.QuotationItem;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationItemController : ControllerBase
    {
        private readonly IQuotationItemService quotationItemService;

        public QuotationItemController(IQuotationItemService quotationItemService)
        {
            this.quotationItemService = quotationItemService;
        }


        [HttpPost]
        public async Task<ActionResult>AddQuotationItem([FromBody] QuotationItemCUDTO dto)
        {
            await quotationItemService.AddQuotationItem(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message ="Quotation item created successfully.",
                Data = null
            });
        }



        [HttpPut]
        public async Task<ActionResult>UpdateQuotationItem([FromBody] QuotationItemCUDTO dto)
        {
            await quotationItemService.UpdateQuotationItem(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message ="Quotation item updated successfully.",
                Data = null
            });
        }



        [HttpDelete("{quotationItemId:int}")]
        public async Task<ActionResult>DeleteQuotationItem(int quotationItemId)
        {
            await quotationItemService.DeleteQuotationItem(quotationItemId);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message ="Quotation item deleted successfully.",
                Data = null
            });
        }

        [HttpGet]
        public async Task<ActionResult>GetAllQuotationItems()
        {
            var data = await quotationItemService.GetAllQuotationItems();

            return Ok(
                new ApiResponse<List<QuotationItemDTO>>
                {
                    Success = true,
                    Message ="Quotation items fetched successfully.",
                    Data = data,
                    TotalNumberRecord = data.Count
                });
        }



        [HttpGet("{quotationItemId:int}")]
        public async Task<ActionResult>GetQuotationItemById(int quotationItemId)
        {
            var data =await quotationItemService.GetQuotationItemById(quotationItemId);

            return Ok(
                new ApiResponse<QuotationItemDTO>
                {
                    Success = true,
                    Message = "Quotation item fetched successfully.",
                    Data = data
                });
        }




        [HttpGet("quotation/{quotationId:int}")]
        public async Task<ActionResult>GetQuotationItemsByQuotationId(int quotationId)
        {
            var data = await quotationItemService.GetQuotationItemsByQuotationId(quotationId);

            return Ok(new ApiResponse<List<QuotationItemDTO>>
                {
                    Success = true,
                    Message ="Quotation items fetched successfully.",
                    Data = data,
                    TotalNumberRecord = data.Count
                });
        }

    }
}
