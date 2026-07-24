using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.API.Controllers
{
    [Route("api/v1/rfq-items")]
    [ApiController]
    public class RFQItemsController : ControllerBase
    {
        private readonly IRFQItemService _rfqItemService;

        public RFQItemsController(IRFQItemService rfqItemService)
        {
            _rfqItemService = rfqItemService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RFQItemCreateDto dto)
        {
            var response = await _rfqItemService.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        // Notice the route here is specifically built for fetching by the parent RFQ ID
        [HttpGet("by-rfq/{rfqId}")]
        public async Task<IActionResult> GetByRfqId(int rfqId)
        {
            var response = await _rfqItemService.GetByRfqIdAsync(rfqId);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RFQItemUpdateDto dto)
        {
            var response = await _rfqItemService.UpdateAsync(id, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _rfqItemService.DeleteAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}