using Backend_Fincore.Application.DTOs.Document;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService service;

        public DocumentController(IDocumentService service)
        {
            this.service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await service.GetAll();

            return Ok(
                new ApiResponse<List<DocumentReadDTO>>
                {
                    Success = true,
                    Message =
                    "Documents fetched successfully.",
                    Data = res,
                    Error = null
                });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var res = await service.GetById(id);


                return Ok(
                    new ApiResponse<DocumentReadDTO>
                    {
                        Success = true,
                        Message =
                        "Document fetched successfully.",
                        Data = res,
                        Error = null
                    });
            }

            catch (Exception ex)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message =
                        "Document not found.",
                        Data = null,
                        Error = ex.Message
                    });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddDocument([FromForm] DocumentWriteDTO dto)
        {
            try
            {
                var res = await service.AddDocument(dto);

                return Ok(
                    new ApiResponse<DocumentReadDTO>
                    {
                        Success = true,
                        Message = "Document created successfully.",
                        Data = res,
                        Error = null
                    });
            }

            catch (Exception ex)
            {
                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation Failed.",
                        Data = null,
                        Error = ex.Message
                    });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, [FromForm] DocumentWriteDTO dto)
        {
            try
            {
                await service.UpdateDocument(id, dto);


                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Document updated successfully.",
                    Data = null,
                    Error = null
                });
            }

            catch (Exception ex)
            {
                if (ex.Message == "Document not found.")
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Document not found.",
                        Data = null,
                        Error = ex.Message
                    });
                }

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Data = null,
                    Error = ex.Message
                });
            }
        }





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                await service.DeleteDocument(id);
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Document deleted successfully.",
                    Data = null,
                    Error = null
                });
            }

            catch (Exception ex)
            {
                return NotFound(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message =
                        "Document not found.",
                        Data = null,
                        Error = ex.Message
                    });
            }
        }
    }
}
