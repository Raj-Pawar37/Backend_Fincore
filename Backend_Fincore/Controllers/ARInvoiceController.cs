using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ARInvoice;
using Backend_Fincore.Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Route("api/[controller]")]
    public class ARInvoiceController : ControllerBase
    {
        private readonly IARInvoiceService _service;
        private readonly IPaymentService _paymentService;

        public ARInvoiceController(
            IARInvoiceService service,
            IPaymentService paymentService)
        {
            _service = service;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var result = await _service.GetAllAsync(pagination);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ARInvoiceCreateDto dto)
        {
            await _service.AddAsync(dto);
            return Ok("Created Successfully");
        }

        [HttpPut]
        public async Task<IActionResult> Update(ARInvoiceUpdateDto dto)
        {
            var result = await _service.UpdateAsync(dto);

            if (!result)
                return NotFound();

            return Ok("Updated Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Deleted Successfully");
        }

        //[HttpPost("{id}/generate-payment")]
        //public async Task<IActionResult> GeneratePayment(int id)
        //{
        //    try
        //    {
        //        await _paymentService.GeneratePaymentFromARInvoice(id);

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Payment generated successfully."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}
    }
}
