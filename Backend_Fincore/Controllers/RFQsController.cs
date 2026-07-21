using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Infrastucture.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RFQsController : ControllerBase
    {
        private readonly IRFQService rfqservice;

        public RFQsController(IRFQService rfqservice)
        {
            this.rfqservice = rfqservice;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response= await rfqservice.GetAll();
            return response.Success ? Ok(response) : BadRequest(response);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id )
        {
            var response = await rfqservice.GetAllById(id);
            return response.Success? Ok(response):BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RFQCreateDto dto)
        {
            var response = await rfqservice.Create(dto);
            return response.Success ? StatusCode(200, response) : StatusCode(500, response);
        }

    }
}
