using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ARInvoiceController : ControllerBase
    {
        private readonly IARInvoiceService _service;

        public ARInvoiceController(IARInvoiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
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
    }
}