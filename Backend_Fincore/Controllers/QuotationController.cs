using Backend_Fincore.Application.DTOs.Quotation;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationService quotationService;

        public QuotationController(IQuotationService quotationService)
        {
            this.quotationService = quotationService;
        }

        [HttpPost]
        public async Task<ActionResult> AddQuotation([FromBody] QuotationCUDTO dto)
        {
            await quotationService.AddQuotation(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Quotation created successfully.",
                Data = null
            });
        }

        [HttpPut]
        public async Task<ActionResult> UpdateQuotation([FromBody] QuotationCUDTO dto)
        {
            await quotationService.UpdateQuotation(dto);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Quotation updated successfully.",
                Data = null
            });
        }

        [HttpDelete("{quotationId:int}")]
        public async Task<ActionResult> DeleteQuotation(int quotationId)
        {
            await quotationService.DeleteQuotation(quotationId);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Quotation deleted successfully.",
                Data = null
            });
        }


        [HttpGet]
        public async Task<ActionResult>GetAllQuotation()
        {
            var data = await quotationService.GetAllQuotation();

            return Ok(new ApiResponse<List<QuotationDTO>>
            {
                Success = true,
                Message = "Quotations fetched successfully.",
                Data = data,
                TotalNumberRecord = data.Count
            });
        }


        [HttpGet("{quotationId:int}")]
        public async Task<ActionResult>GetQuotationById(int quotationId)
        {
            var data = await quotationService.GetQuotationById(quotationId);

            return Ok(new ApiResponse<QuotationDTO>
            {
                Success = true,
                Message = "Quotation fetched successfully.",
                Data = data
            });
        }

        [HttpGet("rfq/{rfqId:int}")]
        public async Task<ActionResult>GetQuotationByRFQId(int rfqId)
        {
            var data = await quotationService.GetQuotationByRFQId(rfqId);

            return Ok(new ApiResponse<List<QuotationDTO>>
            {
                Success = true,
                Message = "RFQ quotations fetched successfully.",
                Data = data,
                TotalNumberRecord = data.Count
            });
        }

    }
}
