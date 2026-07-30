using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Document;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Backend_Fincore.Controllers
{
    [Authorize]
    [Route("api/v1/document")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService service;

        public DocumentController(IDocumentService service)
        {
            this.service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords =await service.GetDocumentCount();
            var totalPages = (int)Math.Ceiling( totalRecords /(double)pagination.PageSize);
            return Ok(
                new ApiResponse<List<DocumentReadDTO>>
                {
                    Success = true,
                    Message =
                    "Documents fetched successfully.",
                    Data = res,
                    Error = null,
                    TotalNumberRecord = totalRecords,
                      Metadata = new
                      {
                          pagination.PageNumber,
                          pagination.PageSize,
                          pagination.Search,
                          TotalPages = totalPages,
                          RecordsOnCurrentPage = res.Count
                      }
                });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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


        [HttpPost]
        public async Task<IActionResult> AddDocument([FromForm] DocumentWriteDTO dto)
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


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(int id, [FromForm] DocumentUpdateDTO dto)
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





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
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
    }
}
