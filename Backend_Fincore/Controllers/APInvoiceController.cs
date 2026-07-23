using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.Interface;
using Backend_Fincore.Service;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class APInvoiceController : ControllerBase
    {
        private readonly IAPInvoiceService aPInvoiceService;

        public APInvoiceController(IAPInvoiceService aPInvoiceService)
        {
            this.aPInvoiceService = aPInvoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllAPInvoice()
        {
            var data = await aPInvoiceService.GetAllAPInvoice();

            return Ok(new ApiResponse<List<APInvoiceDTO>>
            {
                Success = true,
                Message = "AP Invoice fetched successfully.",
                Data = data,
                Error = null
            });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> getAPInvoiceById(int id)
        {
            var data = await aPInvoiceService.GetAPInvoiceById(id);

            return Ok(new ApiResponse<APInvoiceDTO>
            {
                Success = true,
                Message = "AP Invoice fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> addAPInvoice(APInvoiceCUDTO AP)
        {
            await aPInvoiceService.AddAPInvoice(AP);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "AP Invoice created successfully.",
                Data = null,
                Error = null
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateAPInvoice(int id, APInvoiceCUDTO Ap)
        {
            await aPInvoiceService.UpdateAPInvoice(Ap, id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "AP Invoice updated successfully.",
                Data = null,
                Error = null
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteById(int id)
        {
            var data = await aPInvoiceService.DeleteInvoiceById(id);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "APInvoice deleted successfully.",
                Data = null,
                Error = null
            });
        }
    }
}
