using Backend_Fincore.Application.DTOs.DocumentNumber;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.DTOs;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DocumentNumberMasterController : ControllerBase
    {

        IDocumentNumberService _service;
        public DocumentNumberMasterController(IDocumentNumberService service)
        {
            _service = service;
        }

        [HttpGet("generate/{documentName}")]
        public async Task<IActionResult> GenerateDocumentNumber(string documentName)
        {

            string data = await _service.GenerateDocumentNumberAsync(documentName);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Document number generated successfully.",
                Data = data,
                Error = null
            });
        }














        //Basic Crud

        [HttpPost]
        public async Task<IActionResult> DocumentNumber_Create(DocumentNumberCreateTO dto)
        {
            var data = await _service.Create(dto);

            return Ok(new ApiResponse<DocumentNumberDTO>
            {
                Success = true,
                Message = "Document created successfully.",
                Data = data
            });
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> DocumentNumber_Update(int id, DocumentNumberUpdateDTO dto)
        {
            var data = await _service.Update(dto);

            return Ok(new ApiResponse<DocumentNumberDTO>
            {
                Success = true,
                Message = "Document created successfully.",
                Data = data
            });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DocumentNumber_Delete(int id)
        {
            var data = await _service.Delete(id);

            return Ok(new ApiResponse<DocumentNumberDTO>
            {
                Success = true,
                Message = "Document created successfully.",
                Data = null
            });
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> DocumentNumber_ReadById(int id, DocumentNumberUpdateDTO dto)
        {
            var data = await _service.ReadById(id);

            return Ok(new ApiResponse<DocumentNumberDTO>
            {
                Success = true,
                Message = "Document created successfully.",
                Data = data
            });
        }

        [HttpGet]
        public async Task<IActionResult> DocumentNumber_ReadAll()
        {
            var data = await _service.ReadlAll();

            return Ok(new ApiResponse<List<DocumentNumberDTO>>
            {
                Success = true,
                Message = "Document created successfully.",
                Data = data
            });
        }


    }
}
