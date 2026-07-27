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
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService service;
        public DocumentTypeController(IDocumentTypeService service)
        {
            this.service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery]PaginationDTO pagination)
        {
            var res = await service.GetAll(pagination);
            var totalRecords = await service.GetTotalRecordsDocType();
            var totalPages = (int)Math.Ceiling(
                 totalRecords /
                (double)pagination.PageSize);

            return Ok(new ApiResponse<List<DocumentTypeCUDTO>>
            {
                Success = true,
                Message = "DocumentType Masters fetched successfully.",
                Data = res,
                Error = null,
                TotalNumberRecord=totalRecords,
                Metadata = new {
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

                return Ok(new ApiResponse<DocumentTypeCUDTO>
                {
                    Success = true,
                    Message = "DocumentType Master fetched successfully.",
                    Data = res,
                    Error = null
                });
         
        }
        [HttpPost]
        public async Task<IActionResult> AddAccountMaster(DocumentTypeWriteDTO dto)
        {
            var res = await service.AddDocumentType(dto);

            return Ok(new ApiResponse<DocumentTypeCUDTO>
            {
                Success = true,
                Message = "DocumentType Master created successfully.",
                Data = res,
                Error = null
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccountMaster(int id, DocumentTypeUpdateDTO dto)
        {
                await service.UpdateDocumentType(id, dto);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "DocumentType Master updated successfully.",
                    Data = null,
                    Error = null
                });
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountMaster(int id)
        {
           
                await service.DeleteDocumentType(id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "DocumentType Master deleted successfully.",
                    Data = null,
                    Error = null
                });
        }
        [HttpGet("dropdown")]
        public async Task<IActionResult>GetDocumentTypeDropdown(string? search)
        {
            var res = await service.GetDocumentTypeDropdown(search);

            return Ok(
                new ApiResponse<List<DocumentTypeDropdownDTO>>
                {
                    Success = true,
                    Message = "Document Types fetched successfully.",
                    Data = res,
                    Error = null,
                    TotalNumberRecord = res.Count
                });
        }
    }
}
