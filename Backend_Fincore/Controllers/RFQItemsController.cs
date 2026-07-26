using Backend_Fincore.Application.DTOs.RFQItem;
using Backend_Fincore.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Backend_Fincore.API.Controllers
{
    [Authorize]
    [EnableRateLimiting("fixed")]
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
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId") ?? User.FindFirstValue("id");
            int.TryParse(userIdClaim, out int userId);

            var response = await _rfqItemService.CreateAsync(dto, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-rfq/{rfqId}")]
        public async Task<IActionResult> GetByRfqId(int rfqId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _rfqItemService.GetByRfqIdAsync(rfqId, pageNumber, pageSize);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RFQItemUpdateDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId") ?? User.FindFirstValue("id");
            int.TryParse(userIdClaim, out int userId);

            var response = await _rfqItemService.UpdateAsync(id, dto, userId);
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