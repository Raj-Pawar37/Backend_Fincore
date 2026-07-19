using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.Interface;
using Backend_Fincore.Service;
using Backend_Fincore.WrapperClass;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAPInvoiceById(int id)
        {
            var data = await aPInvoiceService.GetAPInvoiceById(id);

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

            return Ok(new ApiResponse<APInvoiceDTO>
            {
                Success = true,
                Message = "AP Invoice fetched successfully.",
                Data = data,
                Error = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddAPInvoice(APInvoiceCUDTO AP)
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
        public async Task<IActionResult> UpdateAPInvoice(int id, APInvoiceCUDTO Ap)
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

            if (!data)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "APInvoice not found.",
                    Data = null,
                    Error = $"No APInvoice found with Id {id}."
                });
            }

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
