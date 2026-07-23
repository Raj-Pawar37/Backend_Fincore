using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_Fincore.API.Controllers
{
    //[Authorize]
    [EnableRateLimiting("fixed")]
    [Route("api/v1/rfqs")]
    [ApiController]
    public class RFQsController : ControllerBase
    {
        private readonly IRFQService _rfqService;

        public RFQsController(IRFQService rfqService)
        {
            _rfqService = rfqService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RFQCreateDto dto)
        {
            var response = await _rfqService.CreateAsync(dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirstValue("UserId") ?? User.FindFirstValue("id");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Success = false, Message = "Invalid or missing token." });
            }

            var response = await _rfqService.GetAllAsync(userId, pageNumber, pageSize);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _rfqService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RFQUpdateDto dto)
        {
            var response = await _rfqService.UpdateAsync(id, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _rfqService.DeleteAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}