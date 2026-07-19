using Backend_Fincore.DTOs;
using Backend_Fincore.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpexRequestController : ControllerBase
    {
        IOpexRequestService service;
        public OpexRequestController(IOpexRequestService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var data = await service.GetAllRequests();
            return Ok(data);

        }
        [HttpPost]
        public async Task<IActionResult> AddOpexRequest(OpexRequestDto dto)
        {
            await service.Create(dto);
            return Ok("Added Successfully ");

        }
        [HttpGet("/{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var dt = await service.GetById(id);
            if (id == null)
            {
                return NotFound();
            }
            return Ok(dt);

        }
        [HttpPut]
        public async Task<IActionResult> UpdateOpexRequest(OpexRequestDto opd)
        {
            await service.Update(opd);
            return Ok("Updated Successfully");

        }
        [HttpDelete("/{id}")]
         public async Task<IActionResult> DeleteOpexRequest(int id)
        {
           await service.Delete(id);
            return Ok("Deleted Successfully");
        }

    }
}
