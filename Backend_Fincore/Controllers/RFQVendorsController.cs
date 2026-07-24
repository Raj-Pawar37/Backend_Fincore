using Backend_Fincore.Application.DTOs.RFQVendor;
using Backend_Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace Backend_Fincore.API.Controllers
{
    //[Authorize]
    [EnableRateLimiting("fixed")]
    [Route("api/v1/rfq-vendors")]
    [ApiController]
    public class RFQVendorsController : ControllerBase
    {
        private readonly IRFQVendorService _rfqVendorService;

        public RFQVendorsController(IRFQVendorService rfqVendorService)
        {
            _rfqVendorService = rfqVendorService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RFQVendorCreateDto dto)
        {
            var response = await _rfqVendorService.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-rfq/{rfqId}")]
        public async Task<IActionResult> GetByRfqId(int rfqId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _rfqVendorService.GetByRfqIdAsync(rfqId, pageNumber, pageSize);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RFQVendorUpdateDto dto)
        {
            var response = await _rfqVendorService.UpdateAsync(id, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _rfqVendorService.DeleteAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
