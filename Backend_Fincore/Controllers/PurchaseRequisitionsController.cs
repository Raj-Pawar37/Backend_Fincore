using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.API.Controllers
{
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
        public async Task<IActionResult> GetAll([FromQuery] int userId)
        {
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

        // Added a specific route for the dropdown so it doesn't conflict with GetById
        [HttpGet("dropdown")]
        public async Task<IActionResult> GetPRDropdown([FromQuery] string? searchText, [FromQuery] int? departmentId)
        {
            var response = await _prService.GetPRDropdownAsync(searchText, departmentId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}