using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims; // Required for JWT extraction

namespace Backend_Fincore.API.Controllers
{
    //[Authorize]
    [EnableRateLimiting("fixed")]
    [Route("api/v1/purchase-requisitions")]
    [ApiController]
    public class PurchaseRequisitionsController : ControllerBase
    {
        private readonly IPurchaseRequisitionService _prService;

        public PurchaseRequisitionsController(IPurchaseRequisitionService prService)
        {
            _prService = prService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            // SECURE: Extract UserId straight from the JWT Token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fallback: If your token generator uses "UserId" or "id" instead of NameIdentifier
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = User.FindFirstValue("UserId") ?? User.FindFirstValue("id");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { Success = false, Message = "Invalid or missing token. Could not extract User ID." });
            }

            var response = await _prService.GetAllAsync(userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _prService.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseRequisitionUpdateDto dto)
        {
            var response = await _prService.UpdateAsync(id, dto);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetPRDropdown([FromQuery] string? searchText, [FromQuery] int? departmentId)
        {
            var response = await _prService.GetPRDropdownAsync(searchText, departmentId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}