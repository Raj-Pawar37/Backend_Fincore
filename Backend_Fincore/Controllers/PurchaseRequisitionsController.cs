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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseRequisitionCreateDto dto)
        {
            var response = await _prService.CreateAsync(dto);
            return response.Success ? StatusCode(200, response) : StatusCode(500, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _prService.GetAllAsync();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _prService.DeleteAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}